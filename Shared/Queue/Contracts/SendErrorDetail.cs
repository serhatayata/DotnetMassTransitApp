namespace Shared.Queue.Contracts;

public class SendErrorDetail
{
    public SendErrorDetail()
    {
    }

    public SendErrorDetail(
        string exception,
        string service)
    {
        this.Exception = exception;
        this.Service = service;
    }

    public string Exception { get; set; }
    public string Service { get; set; }
}
