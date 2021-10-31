using System;

namespace PointOfSale.Results
{
    public class Result<T> : Result
    {
        private readonly T m_value;

        private Result(T value)
        {
            m_value = value;
        }

        private Result(string error) : base(error)
        { }

        public T Value => Success
            ? m_value 
            : throw new InvalidOperationException("Unable to retrieve value of unsuccesfull result.");

        public Result<TOut> Apply<TOut>(Func<T, Result<TOut>> func)
        {
            if (Success) return func(m_value);

            return Result<TOut>.ErrorResult(Error.Value);
        }

        public Result Apply(Func<T, Result> func)
        {
            if (Success) return func(m_value);

            return Result.ErrorResult(Error.Value);
        }

        public static Result<T> SuccessResult(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> ErrorResult(string error)
        {
            return new Result<T>(error);
        }
    }

    public class Result
    {
        protected Result()
        { }

        protected Result(string error)
        {
            Error = new ResultMessage(error);
        }

        public bool Success => Error != default;

        public ResultMessage Error { get; }

        public Result<TOut> Apply<TOut>(Func<Result<TOut>> func)
        {
            if (Success) return func();

            return Result<TOut>.ErrorResult(Error.Value);
        }

        public Result Apply(Func<Result> func)
        {
            if (Success) return func();

            return Result.ErrorResult(Error.Value);
        }

        public static Result ErrorResult(string error)
        {
            return new Result(error);
        }

        public static Result SuccessResult()
        {
            return new Result();
        }
    }
}