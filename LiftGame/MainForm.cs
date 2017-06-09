using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCLNet;
using System.IO;
using System.Drawing.Imaging;
using CL = OpenCLNet;
using System.Threading;

namespace LiftGame
{
	public partial class MainForm : Form
	{ 
		private static Platform platform = null;//平台
		private static Device oclDevice = null;//选中运算设备
		private static Kernel FilterKernel = null;
		private static Context oclContext = null;
		private static CommandQueue oclCQ = null;
		private static Sampler sampler = null;
		private static CL.Image OutImage1 = null, OutImage2 = null;
		private static List<string> CallBackEventList = new List<string>();
		private static Bitmap TestImage = (Bitmap)Bitmap.FromFile(@"01.bmp");

		private static void Main()
		{
			OpenCL.GetPlatformIDs(32, new IntPtr[32], out uint num_platforms);
			List<Device> pt = new List<Device>();
			for (int i = 0; i < num_platforms; pt.AddRange(OpenCL.GetPlatform(i++).QueryDevices(DeviceType.ALL))) ;

			int PT = SelectForm.Show((from Device d in pt select d.Name).ToArray());
			if (PT == -1) return;
			platform = pt[PT].Platform;//平台
			oclDevice = pt[PT];//选中运算设备
			oclContext = platform.CreateContext(new[] { (IntPtr)ContextProperties.PLATFORM, platform.PlatformID, IntPtr.Zero, IntPtr.Zero }, new[] { oclDevice }, new ContextNotify(OpenCLContextNotifyCallBack), IntPtr.Zero);//根据配置建立上下文
			oclCQ = oclContext.CreateCommandQueue(oclDevice, CommandQueueProperties.PROFILING_ENABLE);//创建请求队列
			if (!oclDevice.ImageSupport) return;//如果失败返回
			if (!oclContext.SupportsImageFormat(MemFlags.READ_WRITE, MemObjectType.IMAGE2D, ChannelOrder.RGBA, ChannelType.UNSIGNED_INT8)) return;
			sampler = oclContext.CreateSampler(true, AddressingMode.NONE, FilterMode.NEAREST);

			#region 传入代码编译程序
			string CLCode = @"
bool TestPoint(float x,float y,read_only image2d_t input,sampler_t sampler)
{
	if(x<0||y<0)return false;
	//unsigned char uc=
	return read_imageui( input, sampler, (float2)(x,y) ).x<128;
}

constant uint4 WBGRA=(uint4)(0xFF,0xFF,0xFF,0xFF);
constant uint4 BBGRA=(uint4)(0x00,0x00,0x00,0xFF);
kernel void FilterImage(float outputWidth,float outputHeight,read_only image2d_t input,read_write image2d_t output,sampler_t sampler)
{
	size_t x = get_global_id(0);
	size_t y = get_global_id(1);
	float width = (float)(get_global_size(0)-1);
	float height = (float)(get_global_size(1)-1);

	float sub1x = (x-1)/width;
	float sub1y = (y-1)/height;
	if(x==0)sub1x=-1;
	if(y==0)sub1y=-1;
	float Ix = x/width;
	float Iy = y/height;
	float add1x = (x+1)/width;
	float add1y = (y+1)/height;

	int AX=0;
	if(TestPoint(		sub1x		,		Iy		,input,sampler))AX++;
	if(TestPoint(		sub1x		,		sub1y	,input,sampler))AX++;
	if(TestPoint(		Ix			,		sub1y	,input,sampler))AX++;

	if(y<height)
	{
		if(TestPoint(	sub1x		,		add1y	,input,sampler))AX++;
		if(TestPoint(	Ix			,		add1y	,input,sampler))AX++;
	}
	if(x<width)
	{
		if(TestPoint(	add1x		,		Iy		,input,sampler))AX++;
		if(TestPoint(	add1x		,		sub1y	,input,sampler))AX++;
	}

	if( x < width && y < height )if(TestPoint(add1x,add1y,input,sampler))AX++;

	bool IP=(read_imageui( input, sampler, (float2)(Ix,Iy) ).x<128);
	if(IP)
	{
		write_imageui(output,convert_int2((float2)(x,y)),(AX>1 && AX<4)?BBGRA:WBGRA);
		return;
	}
	write_imageui(output,convert_int2((float2)(x,y)),(AX==3)?BBGRA:WBGRA);
}";
			OpenCLNet.Program oclProgram = oclContext.CreateProgramWithSource(CLCode);
			oclProgram.Build();
			FilterKernel = oclProgram.CreateKernel("FilterImage");
			oclProgram.Dispose();
			#endregion

			OutImage1 = oclContext.CreateImage2D(MemFlags.READ_WRITE, CL.ImageFormat.RGBA8U, TestImage.Width, TestImage.Height, 0, IntPtr.Zero);
			OutImage2 = oclContext.CreateImage2D(MemFlags.READ_WRITE, CL.ImageFormat.RGBA8U, TestImage.Width, TestImage.Height, 0, IntPtr.Zero);

			#region 调用编译好的程序
			FilterKernel.SetArg(0, 1.0f);
			FilterKernel.SetArg(1, 1.0f);
			FilterKernel.SetArg(2, oclContext.ToCLImage(TestImage));
			FilterKernel.SetArg(3, OutImage1);
			FilterKernel.SetArg(4, sampler);
			oclCQ.EnqueueNDRangeKernel(FilterKernel, 2, null, new IntPtr[] { OutImage1.Width, OutImage1.Height, IntPtr.Zero }, null);
			oclCQ.EnqueueBarrier();
			oclCQ.Finish();
			// */
			#endregion

			MainForm MF = new MainForm();
			MF.ShowDialog();
			Application.Exit();
		}

		public static void OpenCLContextNotifyCallBack(string errInfo, byte[] privateInfo, IntPtr cb, IntPtr userData)
		{
			CallBackEventList.Add(errInfo);
			//textBoxCallBackEvents.Lines = CallBackEventList.ToArray();
		}

		public MainForm()
		{
			InitializeComponent();
		}

		private static bool FilterFlag = true;
		private static int FPS = 0;

		private void Filter()
		{
			FilterKernel.SetArg(2, FilterFlag ? OutImage1 : OutImage2);
			FilterKernel.SetArg(3, FilterFlag ? OutImage2 : OutImage1);
			FilterKernel.SetArg(4, sampler);
			oclCQ.EnqueueNDRangeKernel(FilterKernel, 2, null, new IntPtr[] { OutImage1.Width, OutImage1.Height, IntPtr.Zero }, null);
			oclCQ.EnqueueBarrier();
			oclCQ.Finish();
			FilterFlag = !FilterFlag;
			FPS++;
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			Bitmap I = oclContext.ToBitmap(oclCQ, FilterFlag ? OutImage1 : OutImage2);
			e.Graphics.DrawImage(I, 0, 0);
			I.Dispose();
		}

		private void MainForm_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) Filter();
			if (e.Button == MouseButtons.Right) timer1.Enabled = !timer1.Enabled;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			long Ticks = DateTime.Now.Ticks;
			while (true)
			{
				if (DateTime.Now.Ticks > Ticks + 200000) break;
				Filter();
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			this.Refresh();
		}

		private void timer3_Tick(object sender, EventArgs e)
		{
			this.Text = "FPS=" + FPS.ToString();
			FPS = 0;
		}

		private void MainForm_DoubleClick(object sender, EventArgs e)
		{

		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == ' ') timer1.Enabled = !timer1.Enabled;

		}

		private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
		{

		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
}