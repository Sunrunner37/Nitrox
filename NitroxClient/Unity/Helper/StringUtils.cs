﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitroxClient.Unity.Helper
{
    public static class StringUtils
    {
        public static string TruncateRight(this string value, int maxChars, string appendix = "...")
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (appendix == null)
            {
                throw new ArgumentNullException(nameof(appendix));
            }
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + appendix;
        }

        public static string TruncateLeft(this string value, int maxChars, string appendix = "...")
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (appendix == null)
            {
                throw new ArgumentNullException(nameof(appendix));
            }
            return value.Length <= maxChars ? value : appendix + value.Substring(value.Length - maxChars, maxChars);
        }

        public static string ByteArrayToHexString(this byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hex.Append("0x");
                hex.Append(b.ToString("X2"));
                hex.Append(" ");
            }

            return hex.ToString();
        }
    }
}
