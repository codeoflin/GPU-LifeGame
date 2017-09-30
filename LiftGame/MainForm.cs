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
		private static Bitmap TestImage = (Bitmap)System.Drawing.Image.FromFile(@"01.bmp");

		#region Code1
		private static string CLCode1 = @"
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
		#endregion

		#region Code2
		private static string CLCode2 = @"
__kernel void vector_add_gpu(__global int* src_a, __global int* src_b, __global int* res, int num)
{
   const int idx = get_global_id(0);
   if(idx<num)res[idx] =src_a[idx] + src_b[idx];
}";
		#endregion

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
			FilterKernel = oclContext.MakeCode("FilterImage", CLCode1);
			Kernel K2 = oclContext.MakeCode("vector_add_gpu", CLCode2);

			int aaa = K2.PECount(oclDevice);
			aaa = aaa;
			#region 试一下用GPU做运算
			int[] A = new[] { 1, 2, 3, 1722 };
			int[] B = new[] { 456, 2, 1, 56 };
			int[] C = new[] { 0, 0, 0, 0 };
			CL.Mem n1 = oclContext.CreateBuffer(MemFlags.READ_WRITE | MemFlags.COPY_HOST_PTR, A.Length * sizeof(int), A.ToIntPtr());
			CL.Mem n2 = oclContext.CreateBuffer(MemFlags.READ_WRITE | MemFlags.COPY_HOST_PTR, B.Length * sizeof(int), B.ToIntPtr());
			CL.Mem n3 = null;
			unchecked { n3 = oclContext.CreateBuffer(MemFlags.READ_WRITE, B.Length * sizeof(int), IntPtr.Zero); }
			K2.SetArg(0, n1);
			K2.SetArg(1, n2);
			K2.SetArg(2, n3);
			K2.SetArg(3, (int)C.Length);
			oclCQ.EnqueueNDRangeKernel(K2, 1, null, new[] { C.Length, 0 }, null);
			oclCQ.EnqueueBarrier();
			oclCQ.Finish();
			// */
			//oclContext.WriterValues(oclCQ, n3, B);
			C = oclContext.ReadIntValues(oclCQ, n3, C.Length);

			C = C;
			// */
			#endregion

			ShowDeviceInfo SDI = new ShowDeviceInfo();
			ListBox.ObjectCollection lb = SDI.listBox1.Items;
			lb.Add(string.Format("Name:{0}", oclDevice.Name));
			lb.Add(string.Format("DeviceType:{0}", oclDevice.DeviceType.ToString()));
			lb.Add(string.Format("MaxComputeUnits(最大计算单元):{0}", oclDevice.MaxComputeUnits));
			lb.Add(string.Format("ImageSupport:{0}", oclDevice.ImageSupport));
			lb.Add(string.Format("AddressBits:{0}", oclDevice.AddressBits));
			lb.Add(string.Format("DriverVersion:{0}", oclDevice.DriverVersion));
			lb.Add(string.Format("MaxClockFrequency(最大时钟频率):{0}MHz", oclDevice.MaxClockFrequency));
			lb.Add(string.Format("MaxMemAllocSize:{0}", oclDevice.MaxMemAllocSize));
			lb.Add(string.Format("MaxWorkItemDimensions(最大工作维度):{0}", oclDevice.MaxWorkItemDimensions));
			lb.Add(string.Format("MaxWorkGroupSize:{0}", oclDevice.MaxWorkGroupSize));
			lb.Add(string.Format("Version(OpenCL版本):{0}", oclDevice.Version));
			lb.Add(string.Format("GlobalMemSize:{0}", oclDevice.GlobalMemSize));
			lb.Add(string.Format("Vendor(厂商):{0}", oclDevice.Vendor));
			lb.Add(string.Format("HostUnifiedMemory(是否和Host共用内存):{0}", oclDevice.HostUnifiedMemory));
			//SDI.ShowDialog();
			#region 调用编译好的生命游戏程序
			if (oclDevice.DeviceType == DeviceType.GPU)
			{
				OutImage1 = oclContext.CreateImage2D(MemFlags.READ_WRITE, CL.ImageFormat.RGBA8U, TestImage.Width, TestImage.Height, 0, IntPtr.Zero);
				OutImage2 = oclContext.CreateImage2D(MemFlags.READ_WRITE, CL.ImageFormat.RGBA8U, TestImage.Width, TestImage.Height, 0, IntPtr.Zero);
				FilterKernel.SetArg(0, 1.0f);
				FilterKernel.SetArg(1, 1.0f);
				FilterKernel.SetArg(2, oclContext.ToCLImage(TestImage));
				FilterKernel.SetArg(3, OutImage1);
				FilterKernel.SetArg(4, sampler);
				oclCQ.EnqueueNDRangeKernel(FilterKernel, 2, null, new IntPtr[] { OutImage1.Width, OutImage1.Height, IntPtr.Zero }, null);
				oclCQ.EnqueueBarrier();
				oclCQ.Finish();
				new MainForm().ShowDialog();
			}
			// */
			#endregion

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