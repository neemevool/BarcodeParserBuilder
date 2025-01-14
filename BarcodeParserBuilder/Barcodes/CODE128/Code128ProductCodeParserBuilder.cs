﻿using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Exceptions.CODE128;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    internal class Code128ProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
    {
        protected override ProductCode? Parse(string? value) => new Code128ProductCode(value);
        protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj!.Code;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!Code128StringParserBuilder.ValidateFullASCII(value))
                throw new Code128ValidateException($"Invalid Code128 value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode? obj)
        {
            if (obj == null)
                return true;

            if (obj.Type != ProductCodeType.CODE128)
                throw new Code128ValidateException($"Invalid ProductCode type '{obj.Type}'.");

            return true;
        }
    }
}
