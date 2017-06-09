using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OpenCLNet;
using CL = OpenCLNet;

namespace LiftGame
{
	public static class OpenCLExtend
	{
		public static CL.Image ToCLImage(this Context oclContext, Bitmap bitmap)
		{
			CL.Image oclImage;
			BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			oclImage = oclContext.CreateImage2D((MemFlags)((long)MemFlags.READ_ONLY | (long)MemFlags.COPY_HOST_PTR), CL.ImageFormat.RGBA8U, bd.Width, bd.Height, bd.Stride, bd.Scan0);
			bitmap.UnlockBits(bd);
			return oclImage;
		}

		public static Bitmap ToBitmap(this Context oclContext, CommandQueue oclCQ, CL.Image oclBitmap)
		{
			Bitmap bitmap = new Bitmap(oclBitmap.Width.ToInt32(), oclBitmap.Height.ToInt32(), PixelFormat.Format32bppArgb);
			BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			Mem buffer = oclContext.CreateBuffer((MemFlags.WRITE_ONLY | MemFlags.USE_HOST_PTR), bd.Height * bd.Stride, bd.Scan0);
			IntPtr[] origin = new IntPtr[] { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };
			IntPtr[] region = new IntPtr[] { (IntPtr)bitmap.Width, (IntPtr)bitmap.Height, new IntPtr(1) };
			oclCQ.EnqueueCopyImageToBuffer(oclBitmap, buffer, origin, region, IntPtr.Zero);
			oclCQ.EnqueueBarrier();
			IntPtr p = oclCQ.EnqueueMapBuffer(buffer, true, MapFlags.READ, IntPtr.Zero, (IntPtr)(bd.Height * bd.Stride));
			oclCQ.EnqueueUnmapMemObject(buffer, p);
			oclCQ.Finish();
			buffer.Dispose();
			bitmap.UnlockBits(bd);
			return bitmap;
		}

		public static int[] ToInts(this Context oclContext, CommandQueue oclCQ, CL.Mem oclBuff, int Len)
		{
			int[] Ints = new int[Len];
			Mem buffer = oclContext.CreateBuffer((MemFlags.WRITE_ONLY | MemFlags.USE_HOST_PTR), Len * 4, Ints.ToIntPtr());
			oclCQ.EnqueueCopyBuffer(oclBuff, buffer, 0, 0, Len);
			//oclCQ.EnqueueReadBuffer(oclBuff, true, 0, Len, Ints.ToIntPtr());

			oclCQ.EnqueueBarrier();
			IntPtr p = oclCQ.EnqueueMapBuffer(buffer, true, MapFlags.READ, 0, Len);
			oclCQ.EnqueueUnmapMemObject(buffer, p);
			oclCQ.Finish();
			buffer.Dispose();
			return Ints;
		}

		public static float[] Tofloats(this Context oclContext, CommandQueue oclCQ, CL.Mem oclBuff, int Len)
		{
			float[] floats = new float[Len];
			Mem buffer = oclContext.CreateBuffer((MemFlags.WRITE_ONLY | MemFlags.USE_HOST_PTR), Len, floats.ToIntPtr());
			//oclCQ.EnqueueCopyBuffer(oclBuff, buffer, 0, 0, Len);
			oclCQ.EnqueueReadBuffer(oclBuff, true, 0, Len, floats.ToIntPtr());
			oclCQ.EnqueueBarrier();
			IntPtr p = oclCQ.EnqueueMapBuffer(buffer, true, MapFlags.READ, 0, Len);
			oclCQ.EnqueueUnmapMemObject(buffer, p);
			oclCQ.Finish();
			buffer.Dispose();
			return floats;
		}


		public static unsafe IntPtr ToIntPtr(this int[] obj)
		{
			IntPtr PtrA = IntPtr.Zero;
			fixed (int* Ap = obj)
			{
				PtrA = new IntPtr(Ap);
			}
			return PtrA;
		}

		public static unsafe IntPtr ToIntPtr(this float[] obj)
		{
			IntPtr PtrA = IntPtr.Zero;
			fixed (float* Ap = obj)
			{
				PtrA = new IntPtr(Ap);
			}
			return PtrA;
		}
	}//End Class
}
