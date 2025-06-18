namespace Infraestructura.Abstract
{
    public class ResponseEntity<T> : ResponseBase
    {
        public T Data { get; set; }
    }


}
