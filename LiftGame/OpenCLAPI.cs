/*
 * Copyright (c) 2009 Olav Kalgraf(olav.kalgraf@gmail.com)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

// Toggles D3D10 extension support
// NOTE: The C# API is not done yet, but enabling it will let you call something like the raw C API
#define D3D10_Extension

// Toggles Device fission support
// NOTE: The C# API is not done yet, but enabling it will let you call something like the raw C API
#define DEVICE_FISSION_Extension

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace OpenCLNet
{
	#region Using statements

	using cl_int = Int32;
	using cl_uint = UInt32;

	using cl_platform_id = IntPtr;
	using cl_device_id = IntPtr;
	using cl_context = IntPtr;
	using cl_command_queue = IntPtr;
	using cl_mem = IntPtr;
	using cl_program = IntPtr;
	using cl_kernel = IntPtr;
	using cl_event = IntPtr;
	using cl_sampler = IntPtr;

	using cl_bool = UInt32;
	using cl_platform_info = UInt32;
	using cl_device_info = UInt32;
	using cl_command_queue_properties = UInt64;

	using cl_context_properties = IntPtr;
	using cl_context_info = UInt32;
	using cl_command_queue_info = UInt32;
	using cl_mem_flags = UInt64;
	using cl_mem_object_type = UInt32;
	using cl_mem_info = UInt32;
	using cl_image_info = UInt32;
	using cl_addressing_mode = UInt32;
	using cl_filter_mode = UInt32;
	using cl_sampler_info = UInt32;
	using cl_map_flags = UInt64;
	using cl_program_info = UInt32;
	using cl_program_build_info = UInt32;
	using cl_kernel_info = UInt32;
	using cl_kernel_work_group_info = UInt32;
	using cl_event_info = UInt32;
	using cl_profiling_info = UInt32;

	using cl_gl_object_type = UInt32;
	using cl_gl_texture_info = UInt32;
	using GLuint = UInt32;
	using GLint = Int32;
	using GLenum = Int32;

	using cl_d3d10_device_source_khr = UInt32;
	using cl_d3d10_device_set_khr = UInt32;
	using UINT = UInt32;
	using ID3D10Buffer = IntPtr;
	using ID3D10Texture2D = IntPtr;
	using ID3D10Texture3D = IntPtr;

	#endregion


#if D3D10_Extension
	// D3D10 Delegates
	public unsafe delegate ErrorCode clGetDeviceIDsFromD3D10KHRDelegate(cl_platform_id platform, cl_d3d10_device_source_khr d3d_device_source, void* d3d_object, cl_d3d10_device_set_khr d3d_device_set, cl_uint num_entries, cl_device_id* devices, cl_uint* num_devices);
	public unsafe delegate cl_mem clCreateFromD3D10BufferKHRDelegate(cl_context context, cl_mem_flags flags, ID3D10Buffer* resource, out ErrorCode errcode_ret);
	public unsafe delegate cl_mem clCreateFromD3D10Texture2DKHRDelegate(cl_context context, cl_mem_flags flags, ID3D10Texture2D* resource, UINT subresource, out ErrorCode errcode_ret);
	public unsafe delegate cl_mem clCreateFromD3D10Texture3DKHRDelegate(cl_context context, cl_mem_flags flags, ID3D10Texture3D* resource, UINT subresource, out ErrorCode errcode_ret);
	public unsafe delegate ErrorCode clEnqueueAcquireD3D10ObjectsKHRDelegate(cl_command_queue command_queue, cl_uint num_objects, cl_mem* mem_objects, cl_uint num_events_in_wait_list, cl_event* event_wait_list, cl_event* _event);
	public unsafe delegate ErrorCode clEnqueueReleaseD3D10ObjectsKHRDelegate(cl_command_queue command_queue, cl_uint num_objects, cl_mem* mem_objects, cl_uint num_events_in_wait_list, cl_event* event_wait_list, cl_event* _event);
#endif

#if DEVICE_FISSION_Extension
	public delegate ErrorCode clReleaseDeviceEXTDelegate(cl_device_id device);
	public delegate ErrorCode clRetainDeviceEXTDelegate(cl_device_id device);
	public unsafe delegate ErrorCode clCreateSubDevicesEXTDelegate(cl_device_id in_device,
			[In] byte[] properties,
			cl_uint num_entries,
			[In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] cl_device_id[] out_devices,
			[Out] cl_uint* num_devices);
#endif

	/// <summary>
	/// OpenCLAPI - native bindings
	/// </summary>
	[SuppressUnmanagedCodeSecurity()]
	unsafe public static class OpenCLAPI
	{
		internal static class Configuration
		{
			public const string Library = "opencl.dll";
		}

		static OpenCLAPI()
		{

#if D3D10_Extension
			{
				// Get function pointers for D3D10 extension
				IntPtr func;
				func = OpenCLAPI.clGetExtensionFunctionAddress("clGetDeviceIDsFromD3D10KHR");
				if (func != IntPtr.Zero)
					clGetDeviceIDsFromD3D10KHR = (clGetDeviceIDsFromD3D10KHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clGetDeviceIDsFromD3D10KHRDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clCreateFromD3D10BufferKHR");
				if (func != IntPtr.Zero)
					clCreateFromD3D10BufferKHR = (clCreateFromD3D10BufferKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clCreateFromD3D10BufferKHRDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clCreateFromD3D10Texture2DKHR");
				if (func != IntPtr.Zero)
					clCreateFromD3D10Texture2DKHR = (clCreateFromD3D10Texture2DKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clCreateFromD3D10Texture2DKHRDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clCreateFromD3D10Texture3DKHR");
				if (func != IntPtr.Zero)
					clCreateFromD3D10Texture3DKHR = (clCreateFromD3D10Texture3DKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clCreateFromD3D10Texture3DKHRDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clEnqueueAcquireD3D10ObjectsKHR");
				if (func != IntPtr.Zero)
					clEnqueueAcquireD3D10ObjectsKHR = (clEnqueueAcquireD3D10ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clEnqueueAcquireD3D10ObjectsKHRDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clEnqueueReleaseD3D10ObjectsKHR");
				if (func != IntPtr.Zero)
					clEnqueueReleaseD3D10ObjectsKHR = (clEnqueueReleaseD3D10ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clEnqueueReleaseD3D10ObjectsKHRDelegate));
			}
#endif

#if DEVICE_FISSION_Extension
			{
				// Get function pointers for the device fission extension
				IntPtr func;
				func = OpenCLAPI.clGetExtensionFunctionAddress("clReleaseDeviceEXT");
				if (func != IntPtr.Zero)
					clReleaseDeviceEXT = (clReleaseDeviceEXTDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clReleaseDeviceEXTDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clRetainDeviceEXT");
				if (func != IntPtr.Zero)
					clRetainDeviceEXT = (clRetainDeviceEXTDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clRetainDeviceEXTDelegate));
				func = OpenCLAPI.clGetExtensionFunctionAddress("clCreateSubDevicesEXT");
				if (func != IntPtr.Zero)
					clCreateSubDevicesEXT = (clCreateSubDevicesEXTDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clCreateSubDevicesEXTDelegate));
			}
#endif
		}

		#region Platform API

		// Platform API
		[DllImport(Configuration.Library)]
		public extern static cl_int clGetPlatformIDs(cl_uint num_entries, [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] cl_platform_id[] platforms, out cl_uint num_platforms);

		[DllImport(Configuration.Library)]
		public extern static cl_int clGetPlatformInfo(cl_platform_id platform, cl_platform_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region Device API

		// Device APIs
		[DllImport(Configuration.Library)]
		public extern static cl_int clGetDeviceIDs(cl_platform_id platform, [MarshalAs(UnmanagedType.U8)]DeviceType device_type, cl_uint num_entries, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]cl_device_id[] devices, out cl_uint num_devices);

		[DllImport(Configuration.Library)]
		public extern static cl_int clGetDeviceInfo(cl_device_id device, cl_device_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region Context API

		// Context APIs 
		[DllImport(Configuration.Library)]
		public extern static cl_context clCreateContext([In] cl_context_properties[] properties, cl_uint num_devices, [In]cl_device_id[] devices, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_context clCreateContextFromType([In] cl_context_properties[] properties, [MarshalAs(UnmanagedType.U8)]DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_int clRetainContext(cl_context context);
		[DllImport(Configuration.Library)]
		public extern static cl_int clReleaseContext(cl_context context);
		[DllImport(Configuration.Library)]
		public extern static cl_int clGetContextInfo(cl_context context, cl_context_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region Program Object API

		// Program Object APIs
		[DllImport(Configuration.Library)]
		public extern static cl_program clCreateProgramWithSource(cl_context context,
				cl_uint count,
				[In] string[] strings,
				[In] IntPtr[] lengths,
				[MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_program clCreateProgramWithBinary(cl_context context,
				cl_uint num_devices,
				[In] cl_device_id[] device_list,
				[In] IntPtr[] lengths,
				[In] IntPtr[] pBinaries,
				[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] cl_int[] binary_status,
				[MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_int clRetainProgram(cl_program program);
		[DllImport(Configuration.Library)]
		public extern static cl_int clReleaseProgram(cl_program program);
		[DllImport(Configuration.Library)]
		public extern static cl_int clBuildProgram(cl_program program,
				cl_uint num_devices,
				[In] cl_device_id[] device_list,
				string options,
				ProgramNotify pfn_notify,
				IntPtr user_data);
		[DllImport(Configuration.Library)]
		public extern static cl_int clUnloadCompiler();
		[DllImport(Configuration.Library)]
		public extern static cl_int clGetProgramInfo(cl_program program, cl_program_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_int clGetProgramBuildInfo(cl_program program, cl_device_id device, cl_program_build_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region Command Queue API

		// Command Queue APIs
		[DllImport(Configuration.Library)]
		public extern static IntPtr clCreateCommandQueue(cl_context context, cl_device_id device, cl_command_queue_properties properties, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clRetainCommandQueue(cl_command_queue command_queue);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clReleaseCommandQueue(cl_command_queue command_queue);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetCommandQueueInfo(cl_command_queue command_queue, cl_command_queue_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		[Obsolete("Function deprecated in OpenCL 1.1 due to being inherently unsafe", false)]
		public extern static ErrorCode clSetCommandQueueProperty(cl_command_queue command_queue, cl_command_queue_properties properties, [MarshalAs(UnmanagedType.I4)]bool enable, out cl_command_queue_properties old_properties);

		#endregion

		#region Memory Object API

		// Memory Object APIs
		/// <summary>
		/// 创建一个Buffer
		/// </summary>
		/// <param name="context">上下文</param>
		/// <param name="flags">
		/// 1  CL_MEM_READ_WRITE：在device上开辟一段kernal可读可写的内存，这是默认
		/// 2  CL_MEM_WRITE_ONLY：在device上开辟一段kernal只可以写的内存
		/// 3  CL_MEM_READ_ONLY：在device上开辟一段kernal只可以读的内存
		///  
		/// 4  CL_MEM_USE_HOST_PTR：直接使用host上一段已经分配的mem供device使用，注意：这里虽然是用了host上已经存在的内存，但是这个内存的值不一定会和经过kernal函数计算后的实际的值，即使用clEnqueueReadBuffer函数拷贝回的内存和原本的内存是不一样的，或者可以认为opencl虽然借用了这块内存作为cl_mem，但是并不保证同步的，不过初始的值是一样的，（可以使用mapmem等方式来同步）
		/// 5  CL_MEM_ALLOC_HOST_PTR：在host上新开辟一段内存供device使用
		/// 6  CL_MEM_COPY_HOST_PTR：在device上开辟一段内存供device使用，并赋值为host上一段已经存在的mem
		///  
		/// 7  CL_MEM_HOST_WRITE_ONLY:这块内存是host只可写的
		/// 8  CL_MEM_HOST_READ_ONLY:这块内存是host只可读的
		/// 9  CL_MEM_HOST_NO_ACCESS:这块内存是host可读可写的
		/// </param>
		/// <param name="size">长度</param>
		/// <param name="host_ptr">数据指针()只有4和6用到,其它为空</param>
		/// <param name="errcode_ret">错误返回</param>
		/// <returns></returns>
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateBuffer(cl_context context, cl_mem_flags flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateImage2D(cl_context context, cl_mem_flags flags, ImageFormat* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateImage3D(cl_context context, cl_mem_flags flags, ImageFormat* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clRetainMemObject(cl_mem memobj);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clReleaseMemObject(cl_mem memobj);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetSupportedImageFormats(cl_context context,
				cl_mem_flags flags,
				cl_mem_object_type image_type,
				cl_uint num_entries,
				[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ImageFormat[] image_formats,
				out cl_uint num_image_formats);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetMemObjectInfo(cl_mem memobj, cl_mem_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetImageInfo(cl_mem image, cl_image_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		// OpenCL 1.1
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateSubBuffer(cl_mem buffer, cl_mem_flags flags, BufferCreateType buffer_create_type, void* buffer_create_info, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clSetMemObjectDestructorCallback(cl_mem memobj, void* pfn_notify, void* user_data);
		#endregion

		#region Kernel Object API

		// Kernel Object APIs
		[DllImport(Configuration.Library)]
		public extern static cl_kernel clCreateKernel(cl_program program, string kernel_name, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clCreateKernelsInProgram(cl_program program,
				cl_uint num_kernels,
				[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]cl_kernel[] kernels,
				out cl_uint num_kernels_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clRetainKernel(cl_kernel kernel);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clReleaseKernel(cl_kernel kernel);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clSetKernelArg(cl_kernel kernel, cl_uint arg_index, IntPtr arg_size, void* arg_value);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetKernelInfo(cl_kernel kernel, cl_kernel_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetKernelWorkGroupInfo(cl_kernel kernel, cl_device_id device, cl_kernel_work_group_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region Enqueued Commands API

		// Enqueued Commands APIs
		/// <summary>
		/// 从Cl_mem读回host mem（就算Cl_mem是直接使用host mem实现的，想读它的内容，还是要这样读回来，可以看做cl_mem是更高一层封装）.
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="buffer"></param>
		/// <param name="blocking_read"></param>
		/// <param name="offset"></param>
		/// <param name="cb"></param>
		/// <param name="ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		/// <returns></returns>
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_read,
				IntPtr offset,
				IntPtr cb,
				void* ptr,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_read,
				IntPtr offset,
				IntPtr cb,
				void* ptr,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);


		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_write,
				IntPtr offset,
				IntPtr cb,
				void* ptr,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_write,
				IntPtr offset,
				IntPtr cb,
				void* ptr,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBuffer(cl_command_queue command_queue,
				cl_mem src_buffer,
				cl_mem dst_buffer,
				IntPtr src_offset,
				IntPtr dst_offset,
				IntPtr cb,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBuffer(cl_command_queue command_queue,
				cl_mem src_buffer,
				cl_mem dst_buffer,
				IntPtr src_offset,
				IntPtr dst_offset,
				IntPtr cb,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_read,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				IntPtr row_pitch,
				IntPtr slice_pitch,
				void* ptr,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_read,
				IntPtr* origin,
				IntPtr* region,
				IntPtr row_pitch,
				IntPtr slice_pitch,
				void* ptr,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_write,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				IntPtr input_row_pitch,
				IntPtr input_slice_pitch,
				void* ptr,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_write,
				IntPtr* origin,
				IntPtr* region,
				IntPtr input_row_pitch,
				IntPtr input_slice_pitch,
				void* ptr,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyImage(cl_command_queue command_queue,
				cl_mem src_image,
				cl_mem dst_image,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] src_origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] dst_origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyImage(cl_command_queue command_queue,
				cl_mem src_image,
				cl_mem dst_image,
				IntPtr* src_origin,
				IntPtr* dst_origin,
				IntPtr* region,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);


		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyImageToBuffer(cl_command_queue command_queue,
				cl_mem src_image,
				cl_mem dst_buffer,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] src_origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				IntPtr dst_offset,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyImageToBuffer(cl_command_queue command_queue,
				cl_mem src_image,
				cl_mem dst_buffer,
				IntPtr* src_origin,
				IntPtr* region,
				IntPtr dst_offset,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBufferToImage(cl_command_queue command_queue,
				cl_mem src_buffer,
				cl_mem dst_image,
				IntPtr src_offset,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] dst_origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBufferToImage(cl_command_queue command_queue,
				cl_mem src_buffer,
				cl_mem dst_image,
				IntPtr src_offset,
				IntPtr* dst_origin,
				IntPtr* region,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);


		[DllImport(Configuration.Library)]
		public extern static void* clEnqueueMapBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_map,
				cl_map_flags map_flags,
				IntPtr offset,
				IntPtr cb,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static void* clEnqueueMapBuffer(cl_command_queue command_queue,
				cl_mem buffer,
				cl_bool blocking_map,
				cl_map_flags map_flags,
				IntPtr offset,
				IntPtr cb,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event,
				out ErrorCode errcode_ret);

		[DllImport(Configuration.Library)]
		public extern static void* clEnqueueMapImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_map,
				cl_map_flags map_flags,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
				out IntPtr image_row_pitch,
				out IntPtr image_slice_pitch,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static void* clEnqueueMapImage(cl_command_queue command_queue,
				cl_mem image,
				cl_bool blocking_map,
				cl_map_flags map_flags,
				IntPtr* origin,
				IntPtr* region,
				out IntPtr image_row_pitch,
				out IntPtr image_slice_pitch,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event,
				out ErrorCode errcode_ret);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueUnmapMemObject(cl_command_queue command_queue,
				cl_mem memobj,
				void* mapped_ptr,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueUnmapMemObject(cl_command_queue command_queue,
				cl_mem memobj,
				void* mapped_ptr,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);
		#region
		/// <summary>
		/// 执行kernal
		/// </summary>
		/// <param name="command_queue">执行那个device的命令序列</param>
		/// <param name="kernel"></param>
		/// <param name="work_dim"></param>
		/// <param name="global_work_offset"></param>
		/// <param name="global_work_size"></param>
		/// <param name="local_work_size"></param>
		/// <param name="num_events_in_wait_list">说明这个command的执行要等这些event执行了之后</param>
		/// <param name="event_wait_list">说明这个command的执行要等这些event执行了之后</param>
		/// <param name="_event">将返回这个command相关联的event</param>
		/// <returns></returns>
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueNDRangeKernel(cl_command_queue command_queue,
				cl_kernel kernel,
				cl_uint work_dim,
				[In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_offset,
				[In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_size,
				[In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] local_work_size,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		/*
		size_t dim = 2；
		size_t global_offset[] = { 3, 5 };
		size_t global_size[] = { 6, 4 };
		size_t local_size[] = { 3, 2 };
		clEnqueueNDRangeKernel（queue,kernel,dim,global_offset,global_size,local_size,0,NULL,NULL);
		对于上面的参数我们可以通过以下子函数在kernel里获取这些数据：
		uint get_work_dim()：returns the number of dimensions in the kernel's index space
		size_t get_global_size(uint dim): returns the number of work items for a given dimension
		size_t get_global_id(uint dim):returns the element of the work-dim's global ID for a given dimension
		size_t get_global_offset(uint dim):returns the initial offset used to compute global IDs

		size_t get_num_groups(uint dim): returns the number of work-groups for a given dimension
		size_t get_group_id(uint dim):returns the ID of the work-item's work-group for a given dimension
		size_t get_local_id(uint dim): returns the ID of the work-item within its work-group for a given dimension
		size_t get_local_size(uint dim): return the number of work-items in the work-group for a given dimension

		那么我们可以到如下数据：
		uint dim=get_work_dim()lo;//dim=2
		size_t global_id_0=get_global_id(0);//从参数global_offset(3,5)第一个参数3开始，个数为global_size（6,4）的第一参数6
		size_t global_id_1=get_global_id(1);//从参数global_offset(3,5）第二个参数5开始，个数为global_size(6,4)的第二个参数4
		size_t global_size_0=get_global_size(0);//大小为global_size(6,4)的第一个参数6
		size_t global_size_1=get_global_size(1);//大小为global_size(6,4)的第二个参数4
		size_t offset_0=get_global_offset(0);//获取global_offset(3,5)的第一个参数3,
		size_t offset_1=get_global_offset(1);//获取global_offset(3,5)的第二个参数5
		size_t local_id_0=get_local_id(0);//获取local_size(3,2)的第一个参数个数（0,1,2)
		size_t local_id_1=get_local_id(1);//获取local_size(3,2)的第二个参数个数（0,1）
		// */
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="kernel"></param>
		/// <param name="work_dim"></param>
		/// <param name="global_work_offset"></param>
		/// <param name="global_work_size"></param>
		/// <param name="local_work_size"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		/// <returns></returns>
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueNDRangeKernel(cl_command_queue command_queue,
						cl_kernel kernel,
						cl_uint work_dim,
						IntPtr* global_work_offset,
						IntPtr* global_work_size,
						IntPtr* local_work_size,
						cl_uint num_events_in_wait_list,
						IntPtr* event_wait_list,
						cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueTask(cl_command_queue command_queue,
				cl_kernel kernel,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueTask(cl_command_queue command_queue,
				cl_kernel kernel,
				cl_uint num_events_in_wait_list,
				IntPtr* event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueNativeKernel(cl_command_queue command_queue,
				NativeKernelInternal user_func,
				void* args,
				IntPtr cb_args,
				cl_uint num_mem_objects,
				[In] cl_mem[] mem_list,
				[In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] IntPtr[] args_mem_loc,
				cl_uint num_events_in_wait_list,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
				cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueMarker(cl_command_queue command_queue, cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWaitForEvents(cl_command_queue command_queue,
				cl_uint num_events,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_list);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWaitForEvents(cl_command_queue command_queue,
				cl_uint num_events,
				IntPtr* _event_list);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueBarrier(cl_command_queue command_queue);

		// OpenCL 1.1
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadBufferRect(cl_command_queue command_queue,
														cl_mem buffer,
														cl_bool blocking_read,
														[In] IntPtr[] buffer_offset,
														[In] IntPtr[] host_offset,
														[In] IntPtr[] region,
														IntPtr buffer_row_pitch,
														IntPtr buffer_slice_pitch,
														IntPtr host_row_pitch,
														IntPtr host_slice_pitch,
														void* ptr,
														cl_uint num_events_in_wait_list,
														[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
														cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReadBufferRect(cl_command_queue command_queue,
														cl_mem buffer,
														cl_bool blocking_read,
														IntPtr* buffer_offset,
														IntPtr* host_offset,
														IntPtr* region,
														IntPtr buffer_row_pitch,
														IntPtr buffer_slice_pitch,
														IntPtr host_row_pitch,
														IntPtr host_slice_pitch,
														void* ptr,
														cl_uint num_events_in_wait_list,
														cl_event* event_wait_list,
														cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteBufferRect(cl_command_queue command_queue,
														 cl_mem buffer,
														 cl_bool blocking_write,
														 [In] IntPtr[] buffer_offset,
														 [In] IntPtr[] host_offset,
														 [In] IntPtr[] region,
														 IntPtr buffer_row_pitch,
														 IntPtr buffer_slice_pitch,
														 IntPtr host_row_pitch,
														 IntPtr host_slice_pitch,
														 void* ptr,
														 cl_uint num_events_in_wait_list,
														 [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_wait_list,
														 cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueWriteBufferRect(cl_command_queue command_queue,
														 cl_mem buffer,
														 cl_bool blocking_write,
														 IntPtr* buffer_offset,
														 IntPtr* host_offset,
														 IntPtr* region,
														 IntPtr buffer_row_pitch,
														 IntPtr buffer_slice_pitch,
														 IntPtr host_row_pitch,
														 IntPtr host_slice_pitch,
														 void* ptr,
														 cl_uint num_events_in_wait_list,
														 cl_event* _event_wait_list,
														 cl_event* _event);

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBufferRect(cl_command_queue command_queue,
														cl_mem src_buffer,
														cl_mem dst_buffer,
														[In] IntPtr[] src_origin,
														[In] IntPtr[] dst_origin,
														[In] IntPtr[] region,
														IntPtr src_row_pitch,
														IntPtr src_slice_pitch,
														IntPtr dst_row_pitch,
														IntPtr dst_slice_pitch,
														cl_uint num_events_in_wait_list,
														[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_wait_list,
														cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueCopyBufferRect(cl_command_queue command_queue,
														cl_mem src_buffer,
														cl_mem dst_buffer,
														IntPtr* src_origin,
														IntPtr* dst_origin,
														IntPtr* region,
														IntPtr src_row_pitch,
														IntPtr src_slice_pitch,
														IntPtr dst_row_pitch,
														IntPtr dst_slice_pitch,
														cl_uint num_events_in_wait_list,
														cl_event* _event_wait_list,
														cl_event* _event);
		#endregion

		#region Flush and Finish API

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clFlush(cl_command_queue command_queue);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clFinish(cl_command_queue command_queue);

		#endregion

		#region Event Object API

		[DllImport(Configuration.Library)]
		public extern static ErrorCode clWaitForEvents(cl_uint num_events,
				[In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_list);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetEventInfo(cl_event _event,
				cl_event_info param_name,
				IntPtr param_value_size,
				void* param_value,
				out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clRetainEvent(cl_event _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clReleaseEvent(cl_event _event);

		// OpenCL 1.1
		[DllImport(Configuration.Library)]
		public extern static cl_event clCreateUserEvent(cl_context context, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clSetUserEventStatus(cl_event _event, ExecutionStatus execution_status);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clSetEventCallback(cl_event _event, cl_int command_exec_callback_type, EventNotifyInternal pfn_notify, IntPtr user_data);
		#endregion

		#region Sampler API

		// Sampler APIs
		[DllImport(Configuration.Library)]
		public extern static cl_sampler clCreateSampler(cl_context context, cl_bool normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clRetainSampler(cl_sampler sampler);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clReleaseSampler(cl_sampler sampler);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetSamplerInfo(cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

		#endregion

		#region GLObject API

		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateFromGLBuffer(cl_context context,
				cl_mem_flags flags,
				GLuint bufobj,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateFromGLTexture2D(cl_context context,
				cl_mem_flags flags,
				GLenum target,
				GLint mipLevel,
				GLuint texture,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateFromGLTexture3D(cl_context context,
				cl_mem_flags flags,
				GLenum target,
				GLint mipLevel,
				GLuint texture,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static cl_mem clCreateFromGLRenderbuffer(cl_context context,
				cl_mem_flags flags,
				GLuint renderBuffer,
				out ErrorCode errcode_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetGLObjectInfo(cl_mem memobj,
				out cl_gl_object_type gl_object_type,
				out GLuint gl_object_name);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clGetGLTextureInfo(cl_mem memobj,
				cl_gl_texture_info param_name,
				IntPtr param_value_size,
				void* param_value,
				out IntPtr param_value_size_ret);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueAcquireGLObjects(cl_command_queue command_queue,
				cl_uint num_objects,
				[In] cl_mem[] mem_objects,
				cl_uint num_events_in_wait_list,
				[In] cl_event[] event_wait_list,
				cl_event* _event);
		[DllImport(Configuration.Library)]
		public extern static ErrorCode clEnqueueReleaseGLObjects(cl_command_queue command_queue,
				cl_uint num_objects,
				[In] cl_mem[] mem_objects,
				cl_uint num_events_in_wait_list,
				[In] cl_event[] event_wait_list,
				cl_event* _event);

		#endregion


		// Extension function access
		[DllImport(Configuration.Library)]
		public extern static IntPtr clGetExtensionFunctionAddress(string func_name);

		[DllImport(Configuration.Library)]
		public extern static cl_int clGetEventProfilingInfo(cl_event _event, cl_profiling_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

#if D3D10_Extension
		// D3D10 extension
		public static clGetDeviceIDsFromD3D10KHRDelegate clGetDeviceIDsFromD3D10KHR;
		public static clCreateFromD3D10BufferKHRDelegate clCreateFromD3D10BufferKHR;
		public static clCreateFromD3D10Texture2DKHRDelegate clCreateFromD3D10Texture2DKHR;
		public static clCreateFromD3D10Texture3DKHRDelegate clCreateFromD3D10Texture3DKHR;
		public static clEnqueueAcquireD3D10ObjectsKHRDelegate clEnqueueAcquireD3D10ObjectsKHR;
		public static clEnqueueReleaseD3D10ObjectsKHRDelegate clEnqueueReleaseD3D10ObjectsKHR;
#endif

#if DEVICE_FISSION_Extension
		// cl_ext_device_fission extension
		public static clReleaseDeviceEXTDelegate clReleaseDeviceEXT;
		public static clRetainDeviceEXTDelegate clRetainDeviceEXT;
		public static clCreateSubDevicesEXTDelegate clCreateSubDevicesEXT;
#endif
	}

#if false
    unsafe public static class GLSharing
    {
        public delegate cl_int clGetGLContextInfoKHRDelegate(cl_context_properties* properties,
        cl_gl_context_info param_name,
        IntPtr param_value_size,
        void* param_value,
        IntPtr* param_value_size_ret);

        public static readonly clGetGLContextInfoKHRDelegate clGetGLContextInfoKHR;
        static GLSharing()
        {
            IntPtr func = OpenCLAPI.clGetExtensionFunctionAddress("clGetGLContextInfoKHR");
            clGetGLContextInfoKHR = (clGetGLContextInfoKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clGetGLContextInfoKHRDelegate));
        }
    }

    unsafe public static class GLEvent
    {
        static GLEvent()
        {
        }
    }

#endif
}
