﻿namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class GS1128ParseException : ParseException
    {
        public GS1128ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
