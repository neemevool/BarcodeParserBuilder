﻿using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Infrastructure
{
    public class BarcodeDateTimeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidGS1DateStrings))]
        public void CanParseGs1Dates(string value, DateTime expectedDateTime)
        {
            //Arrange & Act
            var result = BarcodeDateTime.Gs1Date(value);

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(expectedDateTime);
            result.StringValue.Should().Be(value);
        }

        public static IEnumerable<object[]> ValidGS1DateStrings()
        {
            //GS1 - 00 day string
            yield return new object[]
            {
                "991200",
                new DateTime(2099, 12, 31)
            };

            //GS1 - Regular Date
            yield return new object[]
            {
                "991231",
                new DateTime(2099, 12, 31)
            };

            //GS1 - Regular Date(February)
            yield return new object[]
            {
                "990200",
                new DateTime(2099, 02, 28)
            };

            //GS1 - Leap-Year
            yield return new object[]
            {
                "200229",
                new DateTime(2020, 02, 29)
            };

            //GS1 - Leap-Year 00 days
            yield return new object[]
            {
                "200200",
                new DateTime(2020, 02, 29)
            };
        }

        [Fact]
        public void CanBuildGs1Dates()
        {
            //Arrange & Act
            var result = BarcodeDateTime.Gs1Date(new DateTime(2020, 12, 24));

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(new DateTime(2020, 12, 24));
            result.StringValue.Should().Be("201224");
        }

        [Theory]
        [InlineData("0991212", "Invalid datetime value '0991212' for format 'yyMMdd'.")] //GS1 - Too long
        [InlineData("99121", "Invalid datetime value '99121' for format 'yyMMdd'.")] //GS1 - Too short
        [InlineData("111#3", "Invalid datetime value '111#3' for format 'yyMMdd'.")] //GS1 - Invalid Character
        [InlineData("NONBER", "Invalid datetime value 'NONBER' for format 'yyMMdd'.")] //GS1 - No Digits
        [InlineData("\0", "Invalid datetime value '\0' for format 'yyMMdd'.")] //GS1 - null character
        public void CanDetectInvalidGs1Dates(string value, string expectedMessage)
        {
            //Arrange & Act
            Action parseDate = () => BarcodeDateTime.Gs1Date(value);

            //Assert
            parseDate.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }


        [Theory]
        [MemberData(nameof(ValidPPNDateStrings))]
        public void CanParsePpnDates(string value, DateTime expectedDateTime)
        {
            //Arrange & Act
            var result = BarcodeDateTime.PpnDate(value);

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(expectedDateTime);
            result.StringValue.Should().Be(value);
        }

        public static IEnumerable<object[]> ValidPPNDateStrings()
        {
            //PPN - 00 day string
            yield return new object[]
            {
                "20991200",
                new DateTime(2099, 12, 31)
            };

            //PPN - Regular Date
            yield return new object[]
            {
                "20991231",
                new DateTime(2099, 12, 31)
            };

            //PPN - Regular Date(February)
            yield return new object[]
            {
                "20990200",
                new DateTime(2099, 02, 28)
            };

            //PPN - Leap-Year
            yield return new object[]
            {
                "20200229",
                new DateTime(2020, 02, 29)
            };

            //PPN - Leap-Year 00 days
            yield return new object[]
            {
                "20200200",
                new DateTime(2020, 02, 29)
            };
        }

        [Fact]
        public void CanBuildPPNDates()
        {
            //Arrange & Act
            var result = BarcodeDateTime.PpnDate(new DateTime(2099, 08, 24));

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(new DateTime(2099, 08, 24));
            result.StringValue.Should().Be("20990824");
        }

        [Theory]
        [InlineData("200912310", "Invalid datetime value '200912310' for format 'yyyyMMdd'.")] //"PPN - Too long
        [InlineData("2009123", "Invalid datetime value '2009123' for format 'yyyyMMdd'.")] //PPN - Too short
        [InlineData("111#3", "Invalid datetime value '111#3' for format 'yyyyMMdd'.")] //PPN - Invalid Character
        [InlineData("NONBER", "Invalid datetime value 'NONBER' for format 'yyyyMMdd'.")] //PPN - No Digits
        [InlineData("\0", "Invalid datetime value '\0' for format 'yyyyMMdd'.")] //PPN - null character
        public void CanDetectInvalidPpnDates(string value, string expectedMessage)
        {
            //Arrange & Act
            Action parseDate = () => BarcodeDateTime.PpnDate(value);

            //Assert
            parseDate.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Theory]
        [MemberData(nameof(ValidHIBCDateStrings))]
        public void CanParseHibcDates(string value, string format, DateTime expectedDateTime)
        {
            //Arrange & Act
            var result = BarcodeDateTime.HibcDate(value, format);

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(expectedDateTime);
            result.StringValue.Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(ValidHIBCDateStrings))]
        public void CanBuildHIBCDates(string expectedValue, string format, DateTime date)
        {
            //Arrange & Act
            var result = BarcodeDateTime.HibcDate(date, format);

            //Assert
            result.Should().NotBeNull();
            result.DateTime.Should().Be(date);
            result.StringValue.Should().Be(expectedValue);
        }

        public static IEnumerable<object[]> ValidHIBCDateStrings()
        {
            //NOTE : DO NOT touch these datetime format indexes. these are static and part of the HIBC specs !
            //therefor, this test also checks the indexes!

            //HIBC - MMYY
            yield return new object[]
            {
                "1299",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                new DateTime(2099, 12, 01)
            };

            //HIBC - MMYY 2
            yield return new object[]
            {
                "0220",
                HibcBarcodeSegmentFormat.SegmentFormats[1],
                new DateTime(2020, 02, 01)
            };

            //HIBC - MMDDYY
            yield return new object[]
            {
                "123199",
                HibcBarcodeSegmentFormat.SegmentFormats[2],
                new DateTime(2099, 12, 31)
            };

            //HIBC - YYMMDD
            yield return new object[]
            {
                "990216",
                HibcBarcodeSegmentFormat.SegmentFormats[3],
                new DateTime(2099, 02, 16)
            };

            //HIBC - YYMMDDHH
            yield return new object[]
            {
                "20022916",
                HibcBarcodeSegmentFormat.SegmentFormats[4],
                new DateTime(2020, 02, 29, 16, 00, 00)
            };

            //HIBC - YYJJJ
            yield return new object[]
            {
                "20059",
                HibcBarcodeSegmentFormat.SegmentFormats[5],
                new DateTime(2020, 02, 28)
            };

            //HIBC - YYJJJ - Leap Year
            yield return new object[]
            {
                "20060",
                HibcBarcodeSegmentFormat.SegmentFormats[5],
                new DateTime(2020, 02, 29)
            };

            //HIBC - YYJJJ - None - Leap Year
            yield return new object[]
            {
                "19060",
                HibcBarcodeSegmentFormat.SegmentFormats[5],
                new DateTime(2019, 03, 01)
            };

            //HIBC - YYJJJHH
            yield return new object[]
            {
                "2005911",
                HibcBarcodeSegmentFormat.SegmentFormats[6],
                new DateTime(2020, 02, 28, 11, 00, 00)
            };

            //HIBC - YYJJJHH - Leap Year
            yield return new object[]
            {
                "2006006",
                HibcBarcodeSegmentFormat.SegmentFormats[6],
                new DateTime(2020, 02, 29, 6, 00, 00)
            };

            //HIBC - YYJJJHH - None - Leap Year
            yield return new object[]
            {
                "1906023",
                HibcBarcodeSegmentFormat.SegmentFormats[6],
                new DateTime(2019, 03, 01, 23, 00, 00)
            };
        }

        [Theory]
        [MemberData(nameof(InvalidHIBCDateStrings))]
        public void CanDetectInvalidHibcDates(string value, string format, string expectedMessage)
        {
            //Arrange & Act
            Action parseDate = () => BarcodeDateTime.HibcDate(value, format);

            //Assert
            parseDate.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        public static IEnumerable<object[]> InvalidHIBCDateStrings()
        {
            //HIBC - Too long
            yield return new object[]
            {
                "12999",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                "Invalid datetime value '12999' for format 'MMyy'."
            };

            //HIBC - Too short
            yield return new object[]
            {
                "129",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                "Invalid datetime value '129' for format 'MMyy'."
            };

            //HIBC - Invalid Character
            yield return new object[]
            {
                "12#9",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                "Invalid datetime value '12#9' for format 'MMyy'."
            };

            //HIBC - No Digits
            yield return new object[]
            {
                "NODI",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                "Invalid datetime value 'NODI' for format 'MMyy'."
            };

            //HIBC - Null Character
            yield return new object[]
            {
                "\0",
                HibcBarcodeSegmentFormat.SegmentFormats[0],
                "Invalid datetime value '\0' for format 'MMyy'."
            };

            //HIBC - No Format
            yield return new object[]
            {
                "1299",
                null,
                "Invalid Hibc date format '(null)'."
            };

            //HIBC - Empty Format
            yield return new object[]
            {
                "1299",
                "",
                "Invalid Hibc date format '(null)'."
            };

            //HIBC - Invalid Format
            yield return new object[]
            {
                "1299",
                "HAHA",
                "Invalid Hibc date format 'HAHA'."
            };

            //HIBC - Devided Format
            yield return new object[]
            {
                "1299",
                "YYMMYYDD",
                "Invalid Hibc date format 'YYMMYYDD'."
            };

            //HIBC - Unknown Format
            yield return new object[]
            {
                "1299",
                "YYYYMMDDHH",
                "Invalid Hibc date format 'YYYYMMDDHH'."
            };
        }
    }
}
