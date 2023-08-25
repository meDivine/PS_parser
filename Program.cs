using PS_parser;
using System;
using System.Threading;

var d = new GetNode();

while(true)
{
    await d.StartThreadsPS5();
    Thread.Sleep(7200000);
}