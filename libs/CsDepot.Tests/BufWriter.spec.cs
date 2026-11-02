//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace CsDepot.Tests;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Clarity over performance in unit tests.")]
public class BufWriterTests
{
    //
    // ── Constructor ─────────────────────────────────────────────────────
    //

    [Fact]
    public void Constructor_StartsEmptyWithNothingWritten()
    {
        BufWriter writer = new();

        Assert.Equal(0, writer.NumWrittenBytes);
        Assert.True(writer.WrittenMemory.IsEmpty);
    }

    //
    // ── WriteBytes ──────────────────────────────────────────────────────
    //

    [Fact]
    public void WriteBytes_AppendsTheBytesAndAdvances()
    {
        BufWriter writer = new();

        writer.WriteBytes(new byte[] { 0x01, 0x02 });

        Assert.Equal(new byte[] { 0x01, 0x02 }, writer.WrittenMemory.ToArray());
        Assert.Equal(2, writer.NumWrittenBytes);
    }


    [Fact]
    public void WriteBytes_WhenCalledAgain_ConcatenatesInOrder()
    {
        BufWriter writer = new();

        writer.WriteBytes(new byte[] { 0x01 });
        writer.WriteBytes(new byte[] { 0x02, 0x03 });

        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, writer.WrittenMemory.ToArray());
        Assert.Equal(3, writer.NumWrittenBytes);
    }


    [Fact]
    public void WriteBytes_WhenGivenNoBytes_WritesNothing()
    {
        BufWriter writer = new();

        writer.WriteBytes(ReadOnlyMemory<byte>.Empty);

        Assert.Equal(0, writer.NumWrittenBytes);
    }

    //
    // ── AlignTo ─────────────────────────────────────────────────────────
    //

    [Theory]
    [InlineData(0, 2, 0)]
    [InlineData(1, 2, 1)]
    [InlineData(2, 2, 0)]
    [InlineData(1, 4, 3)]
    [InlineData(3, 4, 1)]
    [InlineData(4, 4, 0)]
    [InlineData(5, 8, 3)]
    public void
    AlignTo_WritesZeroPadBytesToTheNextBoundary(
        int offsetBytes,
        int boundaryBytes,
        int expectedNumPadBytes
    )
    {
        BufWriter writer = new();
        writer.WriteBytes(new byte[offsetBytes]);

        writer.AlignTo(boundaryBytes);

        Assert.Equal(offsetBytes + expectedNumPadBytes, writer.NumWrittenBytes);
        Assert.Equal(new byte[offsetBytes + expectedNumPadBytes], writer.WrittenMemory.ToArray());
    }


    [Fact]
    public void AlignTo_WhenAlreadyAligned_WritesNothing()
    {
        BufWriter writer = new();
        writer.WriteBytes(new byte[] { 0x01, 0x02 });

        writer.AlignTo(2);

        Assert.Equal(2, writer.NumWrittenBytes);
    }


    [Fact]
    public void AlignTo_WritesPadAfterRealBytes_WithoutDisturbingThem()
    {
        BufWriter writer = new();
        writer.WriteBytes(new byte[] { 0xAA });

        writer.AlignTo(4);

        Assert.Equal(new byte[] { 0xAA, 0x00, 0x00, 0x00 }, writer.WrittenMemory.ToArray());
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void AlignTo_WhenBoundaryIsNotPositive_Throws(int boundaryBytes)
    {
        BufWriter writer = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.AlignTo(boundaryBytes));
    }

    //
    // ── Usage examples ──────────────────────────────────────────────────
    //

    // This test shows the intended way to build a message from several encoded
    // values, inserting a pad byte so a field lands on a 16-bit boundary.  Each
    // WriteBytes would normally take a value's Encode() output.
    [Fact]
    public void Example_WritingSeveralValuesWithPadding()
    {
        BufWriter writer = new();

        writer.WriteBytes(new byte[] { 0x09 });   // a 1-byte header
        writer.AlignTo(2);                         // pad to a 16-bit boundary
        writer.WriteBytes(new byte[] { 0x34, 0x12 });

        Assert.Equal(new byte[] { 0x09, 0x00, 0x34, 0x12 }, writer.WrittenMemory.ToArray());
    }


    // This test shows that BufWriter and BufReader are a matched pair: what one
    // writes, the other reads back.
    [Fact]
    public void Example_RoundTripsThroughBufReader()
    {
        BufWriter writer = new();
        writer.WriteBytes(new byte[] { 0x01, 0x02 });
        writer.AlignTo(4);
        writer.WriteBytes(new byte[] { 0x03 });

        BufReader reader = new(writer.WrittenMemory);

        Assert.Equal(new byte[] { 0x01, 0x02 }, reader.ReadBytes(2).GetValue().ToArray());
        Assert.Equal(new byte[] { 0x00, 0x00 }, reader.Align(4).GetValue().ToArray());
        Assert.Equal(new byte[] { 0x03 }, reader.ReadBytes(1).GetValue().ToArray());
        Assert.Equal(0, reader.NumRemainingBytes);
    }
}
