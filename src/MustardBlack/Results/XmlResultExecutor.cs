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

		public override async Task Execute(PipelineContext context, XmlResult result)
		{
			context.Response.ContentType = "application/xml";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			if (result.Data == null)
				return;

			string xml;

			using (var xmlStream = new MemoryStream())
			{
				var xmlSerializer = new XmlSerializer(result.Data.GetType());
				var xmlTextWriter = new XmlTextWriter(xmlStream, Encoding.UTF8);
				xmlSerializer.Serialize(xmlTextWriter, result.Data);

				var stream = (MemoryStream)xmlTextWriter.BaseStream;
				stream.Position = 0;
				using (var reader = new StreamReader(stream))
				{
					xml = await reader.ReadToEndAsync();
				}
			}

			await context.Response.Write(xml);
		}
	}
}