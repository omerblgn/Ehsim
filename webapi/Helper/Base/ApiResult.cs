namespace webapi.Helper.Base
{
    public class ApiResult
    {
        public ApiResult()
        {
        }

        public ApiResult(bool _Result, string _Message)
        {
            Result = _Result;
            Message = _Message;
        }
        public bool Result { get; set; }
        public string Message { get; set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public ApiResult() { }
        public ApiResult(bool _Result, string _Message, T _Data = default(T))
        {
            Result = _Result;
            Message = _Message;
            Data = _Data;
        }
        public ApiResult(bool _Result, T _Data = default(T))
        {
            Result = _Result;
            Data = _Data;
        }
        public T Data { get; set; }
    }
}


