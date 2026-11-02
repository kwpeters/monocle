//
// At least a portion of the code below was created using AI tool Claude Code.
//

namespace CsDepot.Tests;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Clarity over performance in unit tests.")]
public class BufReaderTests
{
    //
    // ── Constructor ─────────────────────────────────────────────────────
    //

    [Fact]
    public void Constructor_StartsAtOffsetZeroWithAllBytesRemaining()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02, 0x03 });

        Assert.Equal(0, reader.OffsetBytes);
        Assert.Equal(3, reader.NumRemainingBytes);
    }

    //
    // ── ReadBytes ───────────────────────────────────────────────────────
    //

    [Fact]
    public void ReadBytes_WhenEnoughBytesRemain_ReturnsThemAndAdvances()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02, 0x03 });

        ReadOnlyMemory<byte> bytes = reader.ReadBytes(2).ThrowIfError();

        Assert.Equal(new byte[] { 0x01, 0x02 }, bytes.ToArray());
        Assert.Equal(2, reader.OffsetBytes);
        Assert.Equal(1, reader.NumRemainingBytes);
    }


    [Fact]
    public void ReadBytes_WhenCalledAgain_ReadsTheFollowingBytes()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02, 0x03 });
        reader.ReadBytes(1).ThrowIfError();

        Assert.Equal(new byte[] { 0x02, 0x03 }, reader.ReadBytes(2).GetValue().ToArray());
    }


    [Fact]
    public void ReadBytes_WhenNumBytesIsZero_ReturnsNoBytesAndDoesNotAdvance()
    {
        BufReader reader = new(new byte[] { 0x01 });

        Assert.True(reader.ReadBytes(0).GetValue().IsEmpty);
        Assert.Equal(0, reader.OffsetBytes);
    }


    [Fact]
    public void ReadBytes_WhenTooFewBytesRemain_ReturnsErrorAndDoesNotAdvance()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02 });
        reader.ReadBytes(1).ThrowIfError();

        Result<ReadOnlyMemory<byte>, string> result = reader.ReadBytes(2);

        Assert.Equal("Cannot read 2 byte(s) at offset 1. Only 1 are available.", result.GetError());
        Assert.Equal(1, reader.OffsetBytes);
    }


    [Fact]
    public void ReadBytes_WhenNumBytesIsNegative_ReturnsError()
    {
        BufReader reader = new(new byte[] { 0x01 });

        Assert.Equal(
            "Cannot read -1 bytes. The number of bytes must not be negative.",
            reader.ReadBytes(-1).GetError());
        Assert.Equal(0, reader.OffsetBytes);
    }

    //
    // ── Align ───────────────────────────────────────────────────────────
    //

    [Theory]
    [InlineData(0, 2, 0)]
    [InlineData(1, 2, 1)]
    [InlineData(2, 2, 0)]
    [InlineData(1, 4, 3)]
    [InlineData(3, 4, 1)]
    [InlineData(4, 4, 0)]
    [InlineData(5, 8, 3)]
    public void Align_ReadsThePadBytesToTheNextBoundary(
        int offsetBytes,
        int boundaryBytes,
        int expectedNumPadBytes
    )
    {
        BufReader reader = new(new byte[16]);
        reader.ReadBytes(offsetBytes).ThrowIfError();

        ReadOnlyMemory<byte> pad = reader.Align(boundaryBytes).ThrowIfError();

        Assert.Equal(expectedNumPadBytes, pad.Length);
        Assert.Equal(offsetBytes + expectedNumPadBytes, reader.OffsetBytes);
    }


    [Fact]
    public void Align_ReturnsThePadBytes()
    {
        BufReader reader = new(new byte[] { 0x01, 0xAA, 0xBB, 0xCC });
        reader.ReadBytes(1).ThrowIfError();

        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC }, reader.Align(4).GetValue().ToArray());
    }


    // Boundaries are counted from the start of the attempt, even when the
    // attempt is used only to undo reads on failure.
    [Fact]
    public void Align_InsideAnAttempt_CountsBoundariesFromTheAttemptStart()
    {
        BufReader reader = new(new byte[8]);
        reader.ReadBytes(1).ThrowIfError();

        ReadOnlyMemory<byte> pad = reader.Attempt(r =>
        {
            r.ReadBytes(1).ThrowIfError();
            return r.Align(2);
        }).ThrowIfError();

        // Offset 2 is on a 2-byte boundary from the start of the buffer, but
        // the attempt started at offset 1, so it is 1 byte into the attempt.
        Assert.Equal(1, pad.Length);
        Assert.Equal(3, reader.OffsetBytes);
    }


    [Fact]
    public void Align_WhenTooFewPadBytesRemain_ReturnsErrorAndDoesNotAdvance()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02 });
        reader.ReadBytes(1).ThrowIfError();

        Result<ReadOnlyMemory<byte>, string> result = reader.Align(4);

        Assert.Equal(
            "Cannot align to a 4-byte boundary. " +
            "Cannot read 3 byte(s) at offset 1. Only 1 are available.",
            result.GetError());
        Assert.Equal(1, reader.OffsetBytes);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Align_WhenBoundaryIsNotPositive_ReturnsError(int boundaryBytes)
    {
        BufReader reader = new(new byte[] { 0x01 });

        Assert.Equal(
            $"Cannot align to a {boundaryBytes}-byte boundary. The boundary must be positive.",
            reader.Align(boundaryBytes).GetError());
    }

    //
    // ── Attempt ─────────────────────────────────────────────────────────
    //

    [Fact]
    public void Attempt_GivesAReaderAtTheCurrentOffset()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02, 0x03 });
        reader.ReadBytes(1).ThrowIfError();

        int attemptOffsetBytes = reader.Attempt<int>(r => Success(r.OffsetBytes)).GetValue();

        Assert.Equal(1, attemptOffsetBytes);
    }


    [Fact]
    public void Attempt_WhenReadSucceeds_AdvancesPastEverythingRead()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02, 0x03 });

        byte second = reader.Attempt(r =>
        {
            SampleReads.ReadByte(r).ThrowIfError();
            return SampleReads.ReadByte(r);
        }).ThrowIfError();

        Assert.Equal(0x02, second);
        Assert.Equal(2, reader.OffsetBytes);
    }


    [Fact]
    public void Attempt_WhenReadFails_LeavesTheReaderUnchanged()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02 });

        Result<byte, string> result = reader.Attempt(r =>
        {
            SampleReads.ReadByte(r).ThrowIfError();
            SampleReads.ReadByte(r).ThrowIfError();
            return SampleReads.ReadByte(r);
        });

        Assert.True(result.IsError);
        Assert.Equal(0, reader.OffsetBytes);
        Assert.Equal(0x01, SampleReads.ReadByte(reader).GetValue());
    }


    [Fact]
    public void Attempt_WhenReadFails_ReportsOffsetsFromTheBufferStart()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02 });
        reader.ReadBytes(1).ThrowIfError();

        Result<ReadOnlyMemory<byte>, string> result = reader.Attempt(r => r.ReadBytes(2));

        Assert.Equal("Cannot read 2 byte(s) at offset 1. Only 1 are available.", result.GetError());
    }


    [Fact]
    public void Attempt_WhenANestedAttemptFails_UndoesOnlyTheNestedReads()
    {
        BufReader reader = new(new byte[] { 0x01, 0x02 });

        byte first = reader.Attempt(r =>
        {
            Result<byte, string> firstResult = SampleReads.ReadByte(r);
            Result<ReadOnlyMemory<byte>, string> nested = r.Attempt(nr => nr.ReadBytes(2));
            Assert.True(nested.IsError);
            return firstResult;
        }).ThrowIfError();

        Assert.Equal(0x01, first);
        Assert.Equal(1, reader.OffsetBytes);
    }

    //
    // ── Usage examples ──────────────────────────────────────────────────
    //

    // This test shows the intended way to use a reader, rather than testing a
    // single method.  It reads a reply laid out as a status byte, a byte count,
    // and then that many payload bytes:
    // - each from clause reads the next field and, if the read succeeded,
    //   names its value; if it failed, the query stops with its error;
    // - a later field can depend on an earlier one (the count sets the
    //   payload length);
    // - a validation such as RequireZeroStatus is chained onto its read with
    //   Bind; and
    // - Attempt makes a failure anywhere leave the reader where it started.
    [Fact]
    public void Example_ReadingAStructuredReply()
    {
        static Result<byte, string> RequireZeroStatus(byte status) =>
            status == 0 ? Success(status) : Error($"Reply status {status} is not 0.");

        static Result<SampleReply, string> ReadReply(BufReader reader) =>
            reader.Attempt(r =>
                from status in SampleReads.ReadByte(r).Bind(RequireZeroStatus)
                from length in SampleReads.ReadByte(r)
                from payload in r.ReadBytes(length)
                select new SampleReply { Status = status, Payload = payload });

        // Status 0, count 2, payload AA BB, then the first byte of whatever
        // follows the reply.
        BufReader reader = new(new byte[] { 0x00, 0x02, 0xAA, 0xBB, 0x7F });

        SampleReply reply = ReadReply(reader).ThrowIfError();

        Assert.Equal(0x00, reply.Status);
        Assert.Equal(new byte[] { 0xAA, 0xBB }, reply.Payload.ToArray());
        Assert.Equal(4, reader.OffsetBytes);

        // A nonzero status fails the whole reply.
        Assert.Equal(
            "Reply status 1 is not 0.",
            ReadReply(new BufReader(new byte[] { 0x01, 0x02, 0xAA, 0xBB })).GetError());

        // A payload shorter than its count fails the whole reply, and leaves
        // the reader where the reply started.
        BufReader truncatedReader = new(new byte[] { 0x00, 0x05, 0xAA });
        Assert.True(ReadReply(truncatedReader).IsError);
        Assert.Equal(0, truncatedReader.OffsetBytes);
    }


    // This test shows how to read a structure that contains a pad byte.  The
    // structure is a 1-byte tag, a pad byte if needed to reach a 2-byte boundary
    // counted from the start of the structure, and a 2-byte value.  It follows
    // a 1-byte header, so it starts at an odd offset:
    // - the structure reads itself inside Attempt, so Align pads relative to
    //   the start of the structure; and
    // - the structure's read function is called like any other read.
    [Fact]
    public void Example_ReadingAPaddedStructure()
    {
        static Result<SampleTaggedValue, string> ReadTaggedValue(BufReader reader) =>
            reader.Attempt(r =>
                from tag in SampleReads.ReadByte(r)
                from pad in r.Align(2)
                from value in r.ReadBytes(2)
                select new SampleTaggedValue { Tag = tag, Value = value });

        // Header 09 at offset 0.  The structure starts at offset 1: tag 05,
        // then one pad byte (the tag leaves it 1 byte in, which is odd), then
        // the value 34 12 at offsets 3 and 4.  Padding counted from the start
        // of the buffer would see offset 2 as aligned, skip no pad, and read
        // 00 34 as the value.
        BufReader reader = new(new byte[] { 0x09, 0x05, 0x00, 0x34, 0x12 });

        byte header = SampleReads.ReadByte(reader).ThrowIfError();
        SampleTaggedValue body = ReadTaggedValue(reader).ThrowIfError();

        Assert.Equal(0x09, header);
        Assert.Equal(0x05, body.Tag);
        Assert.Equal(new byte[] { 0x34, 0x12 }, body.Value.ToArray());
        Assert.Equal(5, reader.OffsetBytes);
    }
}


// A sample read function, written the way a type that is read from bytes
// would write its own Read method.
file static class SampleReads
{
    public static Result<byte, string> ReadByte(BufReader reader) =>
        reader.ReadBytes(1).MapSuccess(bytes => bytes.Span[0]);
}


// The reply read by BufReaderTests.Example_ReadingAStructuredReply.
file sealed record class SampleReply
{
    public required byte Status { get; init; }
    public required ReadOnlyMemory<byte> Payload { get; init; }
}


// The structure read by BufReaderTests.Example_ReadingAPaddedStructure.
file sealed record class SampleTaggedValue
{
    public required byte Tag { get; init; }
    public required ReadOnlyMemory<byte> Value { get; init; }
}
