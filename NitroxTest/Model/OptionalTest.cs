﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroxModel.DataStructures.Util;

namespace NitroxTest.Model
{
    [TestClass]
    public class OptionalTest
    {
        [TestMethod]
        public void OptionalGet()
        {
            Optional<string> op = Optional.Of("test");
            op.Value.ShouldBeEquivalentTo("test");
        }

        [TestMethod]
        public void OptionalIsPresent()
        {
            Optional<string> op = Optional.Of("test");
            op.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void OptionalIsNotPresent()
        {
            Optional<string> op = Optional.Empty;
            op.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void OptionalOrElseValidValue()
        {
            Optional<string> op = Optional.Of("test");
            op.OrElse("test2").ShouldBeEquivalentTo("test");
        }

        [TestMethod]
        public void OptionalOrElseNoValue()
        {
            Optional<string> op = Optional.Empty;
            op.OrElse("test").ShouldBeEquivalentTo("test");
        }

        [TestMethod]
        public void OptionalEmpty()
        {
            Optional<string> op = Optional.Empty;
            op.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void OptionalValueTypeGet()
        {
            Optional<int> op = Optional.Of(1);
            op.Value.ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public void OptionalValueTypeIsPresent()
        {
            Optional<int> op = Optional.Of(0);
            op.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void OptionalValueTypeIsNotPresent()
        {
            Optional<int> op = Optional.Empty;
            op.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void OptionalValueTypeOrElseValidValue()
        {
            Optional.Of(1).OrElse(2).ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public void OptionalSetValue()
        {
            Optional<int> op;
            op = 1;
            ((int)op).Should().NotBe(0);
        }

        [TestMethod]
        public void OptionalSetValueNull()
        {
            Optional<Random> op = Optional.Of(new Random());
            op.HasValue.Should().BeTrue();
            Action setNull = () => { op = null; };
            setNull.ShouldThrow<ArgumentNullException>("Setting optional to null should not be allowed.");
            op = Optional.Empty;
            op.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void OptionalValueTypeOrElseNoValue()
        {
            Optional<int> op = Optional.Empty;
            op.OrElse(1).ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public void OptionalValueTypeEmpty()
        {
            Optional<int> op = Optional.Empty;
            op.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void OptionalHasValueDynamicChecks()
        {
            Optional.ApplyHasValueCondition<A>(v => v.Threshold <= 200);
            
            Optional<A> a = Optional.Of(new A());
            a.HasValue.Should().BeTrue();
            
            Optional<A> actuallyB = Optional.Of<A>(new B());
            actuallyB.HasValue.Should().BeFalse();
            
            Optional<B> b = Optional.Of(new B());
            b.HasValue.Should().BeFalse();
            
            // If Optional<object> then it should always do all checks because anything can be in it.
            Optional<object> bAsObj = Optional<object>.Of(new B());
            bAsObj.HasValue.Should().BeFalse();
        }

        private class A
        {
            public virtual int Threshold => 200;
        }

        private class B : A
        {
            public override int Threshold => 201;
        }
    }
}
