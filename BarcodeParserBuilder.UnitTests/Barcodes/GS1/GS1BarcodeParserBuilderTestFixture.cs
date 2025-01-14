﻿using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1
{
    public class GS1BarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        public static char GroupSeparator => (char)0x1D;
        public static string GS1DateFormat => "yyMMdd";

        [Theory]
        [MemberData(nameof(ValidGs1Barcodes))]
        [MemberData(nameof(ValidGs1ParsingBarcodes))]
        public void CanParseBarcodeString(string barcode, GS1Barcode expectedBarcode)
        {
            //Arrange & Act
            var parsed = GS1BarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => GS1BarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeTrue();
            parseAction.Should().NotThrow();
            CompareBarcodeObjects(expectedBarcode, result);

            if (expectedBarcode.NetWeightInKg.HasValue)
                result.NetWeightInKg.Value.Should().BeApproximately(expectedBarcode.NetWeightInKg.Value, 0.000001d);
            else
                result.NetWeightInKg.Should().BeNull();

            if (expectedBarcode.NetWeightInPounds.HasValue)
                result.NetWeightInPounds.Value.Should().BeApproximately(expectedBarcode.NetWeightInPounds.Value, 0.000001d);
            else
                result.NetWeightInPounds.Should().BeNull();

            if (expectedBarcode.Price.HasValue)
                result.Price.Value.Should().BeApproximately(expectedBarcode.Price.Value, 0.000000000000001d);
            else
                result.Price.Should().BeNull();

            // Dimension Test
            if (expectedBarcode.Fields["311"].Value != null)
                ((double)result.Fields["311"].Value).Should().BeApproximately((double)expectedBarcode.Fields["311"].Value, 0.000001d);
            else
                result.Fields["311"].Value.Should().BeNull();

            if (expectedBarcode.Fields["312"].Value != null)
                ((double)result.Fields["312"].Value).Should().BeApproximately((double)expectedBarcode.Fields["312"].Value, 0.000001d);
            else
                result.Fields["312"].Value.Should().BeNull();

            if (expectedBarcode.Fields["313"].Value != null)
                ((double)result.Fields["313"].Value).Should().BeApproximately((double)expectedBarcode.Fields["313"].Value, 0.000001d);
            else
                result.Fields["313"].Value.Should().BeNull();

            if (expectedBarcode.Fields["314"].Value != null)
                ((double)result.Fields["314"].Value).Should().BeApproximately((double)expectedBarcode.Fields["314"].Value, 0.000001d);
            else
                result.Fields["314"].Value.Should().BeNull();

        }

        [Theory]
        [MemberData(nameof(ValidGs1Barcodes))]
        public void CanBuildBarcodeString(string expectedBarcode, GS1Barcode barcode)
        {
            //Arrange
            string result = null;

            //Act
            Action parseAction = () => result = GS1BarcodeParserBuilder.Build(barcode);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().Be(expectedBarcode);
        }

        public static IEnumerable<object[]> ValidGs1ParsingBarcodes()
        {
            //Random Order #1
            yield return new object[]
            {
                $"]d220BL0103574661451947301{GroupSeparator}9915489{GroupSeparator}9815647{GroupSeparator}24040600199T{GroupSeparator}71025862471",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = null,
                    SerialNumber = null
                }
            };

            //Random Order #2 (Original Motilium Package)
            yield return new object[]
            {
                $"{GroupSeparator}010357466145194721118165795226{GroupSeparator}17210331101724847.1",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            };

            //GS ending
            yield return new object[]
            {
                $"0134567890123457103456789{GroupSeparator}213456789-012{GroupSeparator}",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012"
                }
            };

            //QR Code Prefix + BatchNumber
            yield return new object[]
            {
                $"]Q30134567890123457103456789",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null
                }
            };

            //DataMatrix Prefix + BatchNumber
            yield return new object[]
            {
                $"]d20134567890123457103456789",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null
                }
            };

            //DotCode Prefix + BatchNumber
            yield return new object[]
            {
                $"]J10134567890123457103456789",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null
                }
            };

            //GS1 example 1
            yield return new object[]
            {
                "]d20108430215011539112212221724022021S3736",
                new GS1Barcode
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("08430215011539", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "843021501153";
                        productCode.Indicator = 0;
                        productCode.Code = "08430215011539";
                    }),
                    SerialNumber = "S3736",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2024, 02, 20), "240220", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2022, 12, 22), "221222", GS1BarcodeParserBuilderTestFixture.GS1DateFormat)
                }
            };

            //GS1 example 2
            var gs1Barcode = new GS1Barcode()
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                {
                    productCode.Type = ProductCodeType.GTIN;
                    productCode.Value = "357466145194";
                    productCode.Indicator = 0;
                }),
                BatchNumber = null,
                SerialNumber = null,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1BarcodeParserBuilderTestFixture.GS1DateFormat)
            };
            gs1Barcode.Fields["20"].SetValue("BL");
            gs1Barcode.Fields["240"].SetValue("40600199T");
            gs1Barcode.Fields["30"].SetValue(1);
            gs1Barcode.Fields["71"].SetValue("025862471");
            gs1Barcode.Fields["98"].SetValue("15647");
            gs1Barcode.Fields["99"].SetValue("15489");
            yield return new object[]
            {
                $"]e00103574661451947110205041799120020BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            };
        }

        public static IEnumerable<object[]> ValidGs1Barcodes()
        {
            var gs1Barcode = new GS1Barcode()
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                {
                    productCode.Type = ProductCodeType.GTIN;
                    productCode.Value = "357466145194";
                    productCode.Indicator = 0;
                }),
                BatchNumber = null,
                SerialNumber = null,
            };
            gs1Barcode.Fields["20"].SetValue("BL");
            gs1Barcode.Fields["240"].SetValue("40600199T");
            gs1Barcode.Fields["30"].SetValue(1);
            gs1Barcode.Fields["71"].SetValue("025862471");
            gs1Barcode.Fields["98"].SetValue("15647");
            gs1Barcode.Fields["99"].SetValue("15489");

            //ProductCode + Unused AI's
            yield return new object[]
            {
                $"010357466145194720BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            };

            //ProductCode
            yield return new object[]
            {
                $"0103574661451947",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = null,
                    SerialNumber = null
                }
            };

            //BatchNumber
            yield return new object[]
            {
                $"0134567890123457103456789",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null
                }
            };

            //SerialNumber
            yield return new object[]
            {
                $"0134567890123457103456789{GroupSeparator}213456789-012",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012"
                }
            };

            //Expiration Date
            yield return new object[]
            {
                $"013456789012345717991200",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = null,
                    SerialNumber = null,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1DateFormat)
                }
            };

            //Production Date
            yield return new object[]
            {
                $"0134567890123457103456789{GroupSeparator}11020504213456789-012",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012",
                    ExpirationDate = null,
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2002,05,04), "020504", GS1DateFormat)
                }
            };

            //Motilium Package (ordered)
            yield return new object[]
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            };

            //UPC-A/EAN12 NDC product in GTIN-14 with indicator 0
            yield return new object[]
            {
                $"0100367457153032101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("00367457153032", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Schema = GtinProductScheme.NationalDrugCode;
                        productCode.Value = "6745715303";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            };

            //UPC-A/EAN12 NDC product in GTIN-14 with indicator 1
            yield return new object[]
            {
                $"0110304094903115101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("10304094903115", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Schema = GtinProductScheme.NationalDrugCode;
                        productCode.Value = "0409490311";
                        productCode.Indicator = 1;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            };

            //NetWeight in Kg
            yield return new object[]
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3105354777",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    NetWeightInKg = 3.54777d
                }
            };

            //NetWeight in Pounds
            yield return new object[]
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3205354777",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    NetWeightInPounds = 3.54777d
                }
            };

            //Price
            yield return new object[]
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3929123456789012345",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    Price = 123456.789012345d
                }
            };

            //Check multiple dimensions
            var gs1BarcodeDimension = new GS1Barcode()
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                {
                    productCode.Type = ProductCodeType.GTIN;
                    productCode.Value = "357466145194";
                    productCode.Indicator = 0;
                }),
                BatchNumber = "1724847.1",
                SerialNumber = "118165795226",
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                ProductionDate = null,
                NetWeightInPounds = 3.54777d
            };
            gs1BarcodeDimension.Fields["311"].SetValue(1.23456); // Length in metres
            gs1BarcodeDimension.Fields["312"].SetValue(1234.56); // Width in metres
            gs1BarcodeDimension.Fields["313"].SetValue(0.01234); // Deepth in metres
            gs1BarcodeDimension.Fields["314"].SetValue(123456d); // Area in square metres

            yield return new object[]
            {
               $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}31151234563122123456313500123431401234563205354777",
                gs1BarcodeDimension
            };
        }

        [Theory]
        [MemberData(nameof(InValidGs1Barcodes))]
        public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange & Act
            var parsed = GS1BarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => GS1BarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
            parseAction.Should()
                .Throw<GS1ParseException>()
                .WithMessage(expectedMessage);
        }

        public static IEnumerable<object[]> InValidGs1Barcodes()
        {
            //ProductCode Too Short
            yield return new object[]
            {
                $"01911972534034{GroupSeparator}103456789",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}01 : Invalid value Length 12. Expected 14 Bytes."
            };

            //Invalid ProductCode
            yield return new object[]
            {
                $"019119725340342717991200213456789-012{GroupSeparator}103456789",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}01 : Invalid GTIN/EAN CheckDigit '7', Expected '8'."
            };

            //Missing AI
            yield return new object[]
            {
                $"0191197253403428ABG3456789-012{GroupSeparator}103456789",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}Invalid character detected in AI 'AB'."
            };

            //Random Character
            yield return new object[]
            {
                $"X019119725340342817991200213456789-012{GroupSeparator}103456789",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}Invalid character detected in AI 'X0'."
            };

            //Invalid Production Date
            yield return new object[]
            {
                $"019119725340342817991200213456789-012{GroupSeparator}103456789{GroupSeparator}110BOGUS",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}11 : Invalid GS1 Date value '0BOGUS'."
            };

            //Invalid Expiration Date
            yield return new object[]
            {
                $"0191197253403428170BOGUS213456789-012{GroupSeparator}103456789{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}17 : Invalid GS1 Date value '0BOGUS'."
            };

            //Invalid Batch String
            yield return new object[]
            {
                $"0191197253403428213456789-012{GroupSeparator}1034#|56789{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}10 : Invalid GS1 string value '34#|56789'."
            };

            //Batch too long
            yield return new object[]
            {
                $"0191197253403428213456789-012{GroupSeparator}10001189998819991197253{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}10 : Invalid value Length 21. Expected Max 20 Bytes."
            };

            //Invalid SerialNumber
            yield return new object[]
            {
                $"01911972534034282134^µ56789{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}21 : Invalid GS1 string value '34^µ56789'."
            };

            //SerialNumber too Long
            yield return new object[]
            {
                $"01911972534034282134567890ABCDE+)97+-ER{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}21 : Invalid value Length 21. Expected Max 20 Bytes."
            };

            //Random Fields Contains Invalid string character
            yield return new object[]
            {
                $"019119725340342899#$^248BFGD^{GroupSeparator}",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}99 : Invalid GS1 string value '#$^248BFGD^'."
            };

            //invalid UPC/EAN
            yield return new object[]
            {
                $"300450549108",
                $"Failed to parse GS1 Barcode :{Environment.NewLine}30 : Invalid value Length 10. Expected Max 8 Bytes."
            };
        }
    }
}
