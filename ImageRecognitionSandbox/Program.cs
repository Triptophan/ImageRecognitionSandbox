using System;
using Brady.ImageRecognition;
using ImageRecognitionSandbox;

namespace ConsoleApplication
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Testing...");
			Console.WriteLine($"{new CloudRecognition().RequestMatch()}");
			//Console.WriteLine($"{new CallTargets().GetTargets()}");
		}
	}
}
