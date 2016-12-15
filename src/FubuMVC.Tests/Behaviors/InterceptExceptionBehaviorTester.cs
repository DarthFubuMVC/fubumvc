﻿using System;
using System.Net;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Behaviors
{
	[TestFixture]
	public class InterceptExceptionBehaviorTester 
	{
		[Test]
		public void should_invoke_inside_behavior()
		{
			var insideBehavior = new DoNothingBehavior();
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = insideBehavior
			};

			cut.Invoke();

			insideBehavior.Invoked.ShouldBeTrue();
		}

		[Test]
		public void when_no_exception_is_thrown_none_should_be_handled()
		{
			var insideBehavior = new DoNothingBehavior();
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = insideBehavior
			};

			cut.Invoke();

			cut.HandledException.ShouldBeNull();
		}

		[Test]
		public void invoke_should_throw_an_exception_when_no_inside_behavior_is_set()
		{
			var interceptExceptionBehavior = new TestInterceptExceptionBehavior<ArgumentException>();

            Exception<FubuAssertionException>.ShouldBeThrownBy(() => interceptExceptionBehavior.Invoke().Wait());
		}

		[Test]
		public void when_matching_exception_is_thrown_by_inside_behavior_it_should_be_handled()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			          {
			          	InsideBehavior = new ThrowingBehavior<ArgumentException>()
			          };

			cut.Invoke();

			cut.HandledException.ShouldBeOfType<ArgumentException>();
		}

		[Test]
		public void when_exception_should_not_be_handled_the_handle_method_should_not_be_invoked()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = new ThrowingBehavior<ArgumentException>()
			};
			cut.SetShouldHandle(false);

			Exception<ArgumentException>.ShouldBeThrownBy(() => cut.Invoke().Wait());

			cut.HandledException.ShouldBeNull();
		}

		[Test]
		public void when_non_matching_exception_is_thrown_should_handled_should_not_be_invoked()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = new ThrowingBehavior<WebException>()
			};

			cut.SetShouldHandle(false);

			Exception<WebException>.ShouldBeThrownBy(() => cut.Invoke().Wait());

			cut.HandledException.ShouldBeNull();
		}
	}

	public class TestInterceptExceptionBehavior<T> : InterceptExceptionBehavior<T> 
		where T : Exception
	{

		private bool shouldHandle = true;
		public T HandledException { get; private set; }
			
		public void SetShouldHandle(bool value)
		{
			shouldHandle = value;	
		}

		public override bool ShouldHandle(T exception)
		{
			return shouldHandle;
		}

		public override void Handle(T exception)
		{
			HandledException = exception;
		}
	}


	public class ThrowingBehavior<T> : IActionBehavior
		where T : Exception, new()
	{
		public Task Invoke()
		{
			throw new T();
		}

		public Task InvokePartial()
		{
			throw new T();
		}
	}

	public class DoNothingBehavior : IActionBehavior
	{
		public bool Invoked { get; set; }

		public Task Invoke()
		{
			Invoked = true;
		    return Task.CompletedTask;
		}

		public Task InvokePartial()
		{
			Invoked = true;
            return Task.CompletedTask;
        }
	}
}