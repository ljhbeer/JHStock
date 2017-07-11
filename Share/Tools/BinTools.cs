using System;
using System.Runtime.InteropServices;

namespace Tools
{
    public class BinTools
    {
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
    
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param>byte数组</param>
        /// <param>结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }
        public static byte[] BytesCopy(byte[] bytSource, byte[] bytTag, int startIndex, int size)
        {
            try
            {
                IntPtr bytePtr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytSource, 0, bytePtr, size);
                Marshal.Copy(bytePtr, bytTag, startIndex, size);
                Marshal.FreeHGlobal(bytePtr);
                return bytTag;
            }
            catch { return null; }
        }
        public static object BytesToStruct(byte[] structObj, int startIndex, int size, Type type)
        {
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(structObj, startIndex, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        public static unsafe Type[] BytesToStruct(byte[] bytes, Type[] type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (bytes.Length % size != 0)
            {
                //返回空
                return null;
            }
            int structlength = bytes.Length / size;
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return type;
        }
    }
}
