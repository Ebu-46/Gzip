using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class MemoryWriteReadExample
{
    private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    private static readonly byte[] s_messageBytes = Encoding.ASCII.GetBytes(Message);
    private static readonly byte[] s_messageBytes2 = Encoding.ASCII.GetBytes(Message);

    public static void Main()
    {
        Console.WriteLine($"The original string length is {s_messageBytes.Length} bytes.");
        using var stream = new MemoryStream();
        CompressBytesToStream(stream);
        Console.WriteLine($"The compressed stream length is {stream.Length} bytes.");
        int decompressedLength = DecompressStreamToBytes(stream);
        Console.WriteLine($"The decompressed string length is {decompressedLength} bytes, same as the original length.");
        /*
         Output:
            The original string length is 445 bytes.
            The compressed stream length is 282 bytes.
            The decompressed string length is 445 bytes, same as the original length.
        */
    }

    private static void CompressBytesToStream(Stream stream)
    {
        using var compressor = new GZipStream(stream, CompressionMode.Compress, true);
        
        var memstream = new MemoryStream(s_messageBytes);
        
        int total_size = (int)memstream.Length;
        int chunk_size = 10;
        int chunk_read_bytes = 1;
        int total_read_bytes = 0;
        byte[] data = new byte[chunk_size];
        
        while(total_read_bytes < total_size && chunk_read_bytes > 0){
            chunk_read_bytes = memstream.Read(data, 0, chunk_size);
            compressor.Write(data, 0, chunk_read_bytes);
            total_read_bytes += chunk_read_bytes;
        }
        //compressor.Write(s_messageBytes, 0, s_messageBytes.Length);
        //compressor.Write(s_messageBytes2, 0, s_messageBytes2.Length);
    }

    private static int DecompressStreamToBytes(Stream stream)
    {
        stream.Position = 0;
        //int bufferSize = 1024;
        
        using var decompressor = new GZipStream(stream, CompressionMode.Decompress);
        byte[] decompressedBytes = new byte[445];
        int length = decompressor.Read(decompressedBytes, 0, 445);
        
        var str = Encoding.ASCII.GetString(decompressedBytes);
        Console.WriteLine(str);
        return length;
    }
}
