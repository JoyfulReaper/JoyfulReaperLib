/*
MIT License

Copyright(c) 2021 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace JoyfulReaperLib.JRXml.RSS.Models
{
	[XmlRoot(ElementName = "channel")]
	public class Channel
	{
		[XmlElement(ElementName = "title")]
		public string Title { get; set; }

		[XmlElement(ElementName = "link")]
		public string Link { get; set; }

		[XmlElement(ElementName = "description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "copyright")]
		public string Copyright { get; set; }

		[XmlElement(ElementName = "language")]
		public string Language { get; set; }

		[XmlElement(ElementName = "managingEditor")]
		public string ManagingEditor { get; set; }

		[XmlElement(ElementName = "webMaster")]
		public string WebMaster { get; set; }

		[XmlElement(ElementName = "category")]
		public string Category { get; set; }

		[XmlElement(ElementName = "generator")]
		public string Generator { get; } = "RSSFeedCreator https://github.com/JoyfulReaper/ProgrammersIdeaBook";

		[XmlElement(ElementName = "docs")]
		public string Docs { get; } = "http://blogs.law.harvard.edu/tech/rss";

		[XmlElement(ElementName = "cloud")]
		public string Cloud { get; set; }

		[XmlElement(ElementName = "ttl")]
		public string Ttl { get; set; }

		[XmlElement(ElementName = "image")]
		public string Image { get; set; }

		[XmlElement(ElementName = "lastBuildDate")]
		public string LastBuildDate { get; set; }

		[XmlElement(ElementName = "item")]
		public List<Item> Items { get; set; } = new List<Item>();

		[XmlElement(ElementName = "textInput")]
		public string TextInput { get; set; }

		[XmlElement(ElementName = "skipHours")]
		public string SkipHours { get; set; }

		[XmlElement(ElementName = "skipDays")]
		public string SkipDays { get; set; }

		[XmlElement(ElementName = "pubDate")]
		public string PubDate { get; set; }
	}
}
