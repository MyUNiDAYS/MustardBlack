using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	public class XmlResultExecutor : ResultExecutor<XmlResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public XmlResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override Task Execute(PipelineContext context, XmlResult result)
		{
			context.Response.ContentType = "application/xml";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			if (result.Data == null)
				return Task.CompletedTask;

			string xml;

			using (var xmlStream = new MemoryStream())
			{
				var xmlSerialiser = new XmlSerializer(result.Data.GetType());
				var xmlTextWriter = new XmlTextWriter(xmlStream, Encoding.UTF8);
				xmlSerialiser.Serialize(xmlTextWriter, result.Data);

				var stream = (MemoryStream)xmlTextWriter.BaseStream;
				stream.Position = 0;
				using (var reader = new StreamReader(stream))
				{
					xml = reader.ReadToEnd();
				}
			}

			return context.Response.Write(xml);
		}
	}
}