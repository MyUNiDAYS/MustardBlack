using MustardBlack.Pipeline;

namespace MustardBlack.Authentication
{
	public static class PipelineContextExtensions
	{
		public static void SetAuthTicket(this PipelineContext context, IAuthTicket ticket)
		{
			context.Items["AuthTicket"] = ticket;
		}

		public static IAuthTicket GetAuthTicket(this PipelineContext context)
		{
			if (!context.Items.ContainsKey("AuthTicket"))
				return new AuthTicket();

			return context.Items["AuthTicket"] as IAuthTicket;
		}
	}
}