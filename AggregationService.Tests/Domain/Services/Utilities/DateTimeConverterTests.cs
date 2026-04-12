namespace AggregationService.Tests.Domain.Services.Utilities
{
    using System;
    using NUnit.Framework;
    using AggregationService.Domain.Services;

    [TestFixture]
    public class DateTimeConverterTests
    {
        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ReturnsNull_WhenInputIsNull()
        {
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ReturnsNull_WhenInputIsEmpty()
        {
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ReturnsNull_WhenInputIsUnparseable()
        {
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("not a date at all");

            Assert.That(result, Is.Null);
        }

        [TestCase("Mon, 07 Jan 2019 12:00:00 -05:00", 17)]
        [TestCase("Mon, 07 Jan 2019 12:00:00 +00:00", 12)]
        [TestCase("Mon, 07 Jan 2019 12:00:00 +02:00", 10)]
        public void ConvertGenericDateTimeOffsetStringToUTC_ConvertsNumericOffset(string input, int expectedUtcHour)
        {
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC(input);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(expectedUtcHour));
            Assert.That(result.Value.Date, Is.EqualTo(new DateTime(2019, 1, 7)));
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ConvertsEST()
        {
            // EST is -05:00, so 12:00 EST = 17:00 UTC
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("Mon, 07 Jan 2019 12:00:00 EST");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(17));
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ConvertsPST()
        {
            // PST is -08:00, so 08:00 PST = 16:00 UTC
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("Mon, 07 Jan 2019 08:00:00 PST");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(16));
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ConvertsGMT()
        {
            // GMT is +00:00
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("Mon, 07 Jan 2019 12:00:00 GMT");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(12));
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_ConvertsCEST()
        {
            // CEST is +02:00, so 14:00 CEST = 12:00 UTC
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("Mon, 07 Jan 2019 14:00:00 CEST");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(12));
        }

        [Test]
        public void ConvertGenericDateTimeOffsetStringToUTC_HandlesTrailingWhitespace()
        {
            var result = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC("Mon, 07 Jan 2019 12:00:00 EST  ");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(result.Value.Hour, Is.EqualTo(17));
        }
    }
}
