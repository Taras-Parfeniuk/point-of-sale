using System;
using NUnit.Framework;

namespace PointOfSale.Results.Tests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void SuccessResult_ShouldApplyFunction()
        {
            var funcApplied = false;
            var target = Result.SuccessResult();
            Func<Result> funcToApply = 
                () => 
                { 
                    funcApplied = true; 
                    return Result.SuccessResult(); 
                };

            target.Apply(funcToApply);

            Assert.IsTrue(funcApplied);
        }

        [Test]
        public void ErrorResult_ShouldNotApplyFunction()
        {
            var funcApplied = false;
            var target = Result.ErrorResult(string.Empty);
            Func<Result> funcToApply =
                () =>
                {
                    funcApplied = true;
                    return Result.SuccessResult();
                };

            var result = target.Apply(funcToApply);

            Assert.IsFalse(funcApplied);
        }

        public void ErrorResult_ShouldReturnErrorResult()
        {
            var target = Result.ErrorResult(string.Empty);
            Func<Result> funcToApply = () => Result.SuccessResult();

            var result = target.Apply(funcToApply);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void SuccessTypedResult_ShouldApplyFunctionWithResultValueAsParam()
        {
            var funcApplied = false;
            var target = Result<Guid>.SuccessResult(Guid.NewGuid());
            Func<Guid, Result<Guid>> funcToApply =
                (value) =>
                {
                    funcApplied = true;
                    return Result<Guid>.SuccessResult(value);
                };

            var result = target.Apply(funcToApply);

            Assert.IsTrue(funcApplied);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(target.Value, result.Value);
        }

        [Test]
        public void ErrorTypedResult_ShouldThrowAnExceptionOnGetValue()
        {
            var target = Result<Guid>.ErrorResult(string.Empty);
            Action funcToThrowException = () => { var result = target.Value; };
            
            Assert.Throws(typeof(InvalidOperationException), new TestDelegate(funcToThrowException));
        }
    }
}
