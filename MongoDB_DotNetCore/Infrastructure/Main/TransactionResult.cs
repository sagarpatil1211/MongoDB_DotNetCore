using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class TransactionResult
{
	public bool Successful = false;
	public string Message = string.Empty;
	public string Tag = string.Empty;
	public string TagType = string.Empty;
	public string ProcessToken = string.Empty;

	public TransactionResult() {}

	public static TransactionResult FromPayloadPacket(PayloadPacket pkt)
	{
		return JsonConvert.DeserializeObject<TransactionResult>(Utils.GetString(pkt.Payload));
	}

	public virtual string Serialize()
	{
		return JsonConvert.SerializeObject(this);
	}

	public void AbsorbException(Exception ex)
	{
		Successful = false;
		Message = ex.Message;
	}

	public void AbsorbExceptionWithStackTrace(Exception ex)
	{
		Successful = false;
		Message = $"{ex.Message}\n\nSTACK TRACE:\n{ex.StackTrace}";
	}
}

public class TransactionResult<T> : TransactionResult
{
	public TransactionResult() : base() {}

	public T Data;

	public override string Serialize()
	{
		return JsonConvert.SerializeObject(this);
	}
}