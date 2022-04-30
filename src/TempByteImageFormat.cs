using System;

/// <summary>
/// Temp byte based image format. 0 is zero color, 255 is max color. Channels per color can be defined
/// </summary>
public sealed class TempByteImageFormat : IImageFormat<byte>
{
	/// <summary>
	/// Width of bitmap
	/// </summary>
	public readonly int width;

	/// <summary>
	/// Height of bitmap
	/// </summary>
	public readonly int height;

	private readonly byte[] content;

	/// <summary>
	/// How many color channels per pixel
	/// </summary>
	public readonly int channelsPerPixel;

	/// <summary>
	/// Constructor for temp byte image format
	/// </summary>
	/// <param name="input">Input byte array</param>
	/// <param name="imageWidth">Width</param>
	/// <param name="imageHeight">Height</param>
	/// <param name="imageChannelsPerPixel">Image channels per pixel</param>
	/// <param name="createCopy">True if you want to create copy of data</param>
	public TempByteImageFormat(byte[] input, int imageWidth, int imageHeight, int imageChannelsPerPixel, bool createCopy = false)
	{
		if (createCopy)
		{
			this.content = new byte[input.Length];
			Buffer.BlockCopy(input, 0, this.content, 0, input.Length);
		}
		else
		{
			this.content = input;
		}
		this.width = imageWidth;
		this.height = imageHeight;
		this.channelsPerPixel = imageChannelsPerPixel;
	}

	/// <summary>
	/// Constructor for temp byte image format
	/// </summary>
	/// <param name="input">Existing TempByteImageFormat</param>
	public TempByteImageFormat(TempByteImageFormat input)
	{
		this.content = input.content;
		this.width = input.width;
		this.height = input.height;
		this.channelsPerPixel = input.channelsPerPixel;
	}

	/// <summary>
	/// Get width of bitmap
	/// </summary>
	/// <returns>Width in pixels</returns>
	public int GetWidth()
	{
		return this.width;
	}    
	
	/// <summary>
	/// Get height of bitmap
	/// </summary>
	/// <returns>Height in pixels</returns>
	public int GetHeight()
	{
		return this.height;
	}

	/// <summary>
	/// Get channels per pixel
	/// </summary>
	/// <returns>Channels per pixel</returns>
	public int GetChannelsPerPixel()
	{
		return this.channelsPerPixel;
	}

	/// <summary>
	/// Get raw content as byte array
	/// </summary>
	/// <returns>Byte array</returns>
	public byte[] GetRawContent() => this.content;

	/// <summary>
	/// Set pixel channels of certain coordinate
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="newValues">New values as object array</param>
	public void SetPixelChannels(int x, int y, byte[] newValues)
	{
		int indexBase = y * this.width * this.channelsPerPixel + x * this.channelsPerPixel;
		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			this.content[indexBase + i] = newValues[i];
		}
	}

	/// <summary>
	/// Get pixel channels of certain coordinate
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <returns>Values as byte array</returns>
	public byte[] GetPixelChannels(int x, int y)
	{
		byte[] returnArray = new byte[this.channelsPerPixel];
		
		int indexBase = y * this.width * this.channelsPerPixel + x * this.channelsPerPixel;
		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			returnArray[i] = this.content[indexBase + i];
		}

		return returnArray;
	}

	/// <summary>
	/// Get pixel channels of certain coordinate
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="pixelStorage">Array where pixel channels values will be written</param>
	public void GetPixelChannels(int x, int y, ref byte[] pixelStorage)
	{
		int indexBase = y * this.width * this.channelsPerPixel + x * this.channelsPerPixel;
		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			pixelStorage[i] = this.content[indexBase + i];
		}
	}

	/// <summary>
	/// Get quantization errors per channel
	/// </summary>
	/// <param name="originalPixel">Original pixels</param>
	/// <param name="newPixel">New pixels</param>
	/// <returns>Error values as object array</returns>
	public double[] GetQuantErrorsPerChannel(byte[] originalPixel, byte[] newPixel)
	{
		double[] returnValue = new double[this.channelsPerPixel];

		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			returnValue[i] = originalPixel[i] - newPixel[i];
		}

		return returnValue;
	}

	/// <summary>
	/// Get quantization errors per channel
	/// </summary>
	/// <param name="originalPixel">Original pixels</param>
	/// <param name="newPixel">New pixels</param>
	/// <param name="errorValues">Error values as double array</param>
	public void GetQuantErrorsPerChannel(in byte[] originalPixel, in byte[] newPixel, ref double[] errorValues)
	{
		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			errorValues[i] = originalPixel[i] - newPixel[i];
		}
	}

	/// <summary>
	/// Modify existing values with quantization errors
	/// </summary>
	/// <param name="modifyValues">Values to modify</param>
	/// <param name="quantErrors">Quantization errors</param>
	/// <param name="multiplier">Multiplier</param>
	public void ModifyPixelChannelsWithQuantError(ref byte[] modifyValues, double[] quantErrors, double multiplier)
	{
		for (int i = 0; i < this.channelsPerPixel; i++)
		{
			modifyValues[i] = GetLimitedValue((byte)modifyValues[i], quantErrors[i] * multiplier);
		}
	}

	private static byte GetLimitedValue(byte original, double error)
	{
		double newValue = original + error;
		return Clamp(newValue, byte.MinValue, byte.MaxValue);
	}

	// C# doesn't have a Clamp method so we need this
	private static byte Clamp(double value, double min, double max)
	{
		return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
	}
}