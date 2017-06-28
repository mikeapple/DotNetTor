﻿using DotNetTor.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static DotNetTor.Http.Constants;

namespace DotNetTor.Http
{
    public abstract class StartLine
	{
		public HttpProtocol Protocol { get; protected set; }
		public string StartLineString { get; protected set; }
		public List<string> Parts => GetParts(StartLineString);

		public override string ToString()
		{
			return StartLineString;
		}

		public static List<string> GetParts(string startLineString)
		{
			var trimmed = "";
			// check if an unexpected crlf in the startlinestring
			using (var reader = new StringReader(startLineString))
			{
				// read to CRLF, if it doesn't end with that it reads to end, if it does, it removes it
				trimmed = reader.ReadLine(strictCRLF: true);
				// startLineString must end here
				if (reader.Read() != -1)
				{
					throw new Exception($"Wrong {startLineString} provided");
				}
			}

			var parts = new List<string>();
			using(var reader = new StringReader(trimmed))
			{
				while (true)
				{
					var part = reader.ReadPart(SP.ToCharArray()[0]);

					if(part == null || part == "")
					{
						break;
					}

					if (parts.Count == 2)
					{
						if (part == null)
						{
							throw new Exception($"Wrong {startLineString} provided");
						}

						var rest = reader.ReadToEnd();

						// startLineString must end here, the ReadToEnd returns "" if nothing to read instead of null
						if(rest != "")
						{
							part += SP + rest;
						}						
					}
					parts.Add(part);					
				}
			}
			return parts;
		}
	}
}