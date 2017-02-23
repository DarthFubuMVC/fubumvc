﻿using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Behaviors
{
	
	public class InterceptExceptionBehaviorTester 
	{
		[Fact]
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

		[Fact]
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

		[Fact]
		public void invoke_should_throw_an_exception_when_no_inside_behavior_is_set()
		{
			var interceptExceptionBehavior = new TestInterceptExceptionBehavior<ArgumentException>();

            Exception<FubuAssertionException>.ShouldBeThrownBy(interceptExceptionBehavior.Invoke);
		}

		[Fact]
		public void when_matching_exception_is_thrown_by_inside_behavior_it_should_be_handled()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			          {
			          	InsideBehavior = new ThrowingBehavior<ArgumentException>()
			          };

			cut.Invoke();

			cut.HandledException.ShouldBeOfType<ArgumentException>();
		}

		[Fact]
		public void when_exception_should_not_be_handled_the_handle_method_should_not_be_invoked()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = new ThrowingBehavior<ArgumentException>()
			};
			cut.SetShouldHandle(false);

			Exception<ArgumentException>.ShouldBeThrownBy(cut.Invoke);

			cut.HandledException.ShouldBeNull();
		}

		[Fact]
		public void when_non_matching_exception_is_thrown_should_handled_should_not_be_invoked()
		{
			var cut = new TestInterceptExceptionBehavior<ArgumentException>
			{
				InsideBehavior = new ThrowingBehavior<WebException>()
			};
			cut.SetShouldHandle(false);

			Exception<WebException>.ShouldBeThrownBy(cut.Invoke);

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
		public void Invoke()
		{
			throw new T();
		}

		public void InvokePartial()
		{
			throw new T();
		}
	}

	public class DoNothingBehavior : IActionBehavior
	{
		public bool Invoked { get; set; }

		public void Invoke()
		{
			Invoked = true;
		}

		public void InvokePartial()
		{
			Invoked = true;
		}
	}
}