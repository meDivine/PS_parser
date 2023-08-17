using HtmlAgilityPack;
using PS_parser;
using PS_parser.Sheets;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


var d = new GetNode();

await d.StartThreadsPS5();
