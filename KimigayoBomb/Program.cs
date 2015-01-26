using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KimigayoBomb
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2) return;

			var xml = new XmlSerializer(typeof(tv));
			var sr = new StreamReader(args[0], Encoding.UTF8);
			var epg0 = (tv)xml.Deserialize(sr);
			sr.Close();
			sr = null;

			sr = new StreamReader(args[1], Encoding.UTF8);
			var epg1 = (tv)xml.Deserialize(sr);
			sr.Close();
			sr = null;

			var epg = new tv()
			{
				programme = new List<tvProgramme>(),
				channel = new List<tvChannel>()
			};
			epg.programme = epg.programme.Union(epg0.programme).ToList();
			epg.programme = epg.programme.Union(epg1.programme).ToList();
			epg.channel = epg.channel.Union(epg0.channel).ToList();
			epg.channel = epg.channel.Union(epg1.channel).ToList();


			var bombList = epg.programme
				.Where(x => x.title[0].Value == "放送休止")
				.Select(x =>
				{
					var y = new BombSchedule
					{
						Channnel = epg.channel.Find(channel => channel.id == x.channel).displayname[0].Value,
						Start = DateTime.ParseExact(x.start, "yyyyMMddHHmmss K", null),
						End = DateTime.ParseExact(x.stop, "yyyyMMddHHmmss K", null)
					};
					return y;
				})
				.OrderBy(x => x.Start);
			foreach (var tvProgramme in bombList)
			{
				Console.WriteLine("Ch." + tvProgramme.Channnel);
				Console.WriteLine("Start:" + tvProgramme.Start);
				Console.WriteLine("End:" + tvProgramme.End);
				Console.WriteLine("予想起立時刻: " + (tvProgramme.Start + new TimeSpan(0, 0, 20)));
				Console.WriteLine("予想着席時刻: " + (tvProgramme.Start + new TimeSpan(0, 0, 78)));
				Console.WriteLine();
			}
			Console.Write(bombList);
		}

		class BombSchedule
		{
			public string Channnel { get; set; }
			public DateTime Start { get; set; }
			public DateTime End { get; set; }
		}
	}
}
