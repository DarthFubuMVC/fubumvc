using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class PropertyChainTester
    {
        [SetUp]
        public void SetUp()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            PropertyInfo second = ReflectionHelper.GetProperty<ChildTarget>(x => x.GrandChild);
            PropertyInfo birthday = ReflectionHelper.GetProperty<GrandChildTarget>(x => x.BirthDay);

            _chain = new PropertyChain(new[]
                                       {
                                           new PropertyValueGetter(top),
                                           new PropertyValueGetter(second),
                                           new PropertyValueGetter(birthday),

                                       });
        }

        [Test]
        public void prepend_property()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            var accessor = ReflectionHelper.GetAccessor<ChildTarget>(x => x.GrandChild.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target(){
                Child = new ChildTarget(){
                    GrandChild = new GrandChildTarget(){
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldEqual(new DateTime(1974, 1, 1));
        }

        [Test]
        public void prepend_accessor()
        {
            var top = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            var accessor = ReflectionHelper.GetAccessor<ChildTarget>(x => x.GrandChild.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target()
            {
                Child = new ChildTarget()
                {
                    GrandChild = new GrandChildTarget()
                    {
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldEqual(new DateTime(1974, 1, 1));
        }

        [Test]
        public void prepend_accessor_2()
        {
            var top = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild);
            var accessor = ReflectionHelper.GetAccessor<GrandChildTarget>(x => x.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target()
            {
                Child = new ChildTarget()
                {
                    GrandChild = new GrandChildTarget()
                    {
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldEqual(new DateTime(1974, 1, 1));
        }

        private PropertyChain _chain;

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
        }

        public class ChildTarget
        {
            public ChildTarget()
            {
                GrandChildren = new List<GrandChildTarget>();
            }

            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }
            public IList<GrandChildTarget> GrandChildren { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
        }

        [Test]
        public void to_expression_test()
        {
            var target = new Target{
                Child = new ChildTarget(){
                    GrandChild = new GrandChildTarget(){
                        Name = "Jessica"
                    },
                    SecondGrandChild = new GrandChildTarget(){
                        Name = "Natalie"
                    }
                }
            };

            ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.Name)
                .ToExpression<Target>().Compile()(target)
                .ShouldEqual("Jessica");

            ReflectionHelper.GetAccessor<Target>(x => x.Child.SecondGrandChild.Name)
                .ToExpression<Target>().Compile()(target)
                .ShouldEqual("Natalie");
        }

        [Test]
        public void to_expression_test_with_a_value_type()
        {
            var target = new Target
                         {
                             Child = new ChildTarget()
                                     {
                                         GrandChild = new GrandChildTarget()
                                                      {
                                                          Number = 1
                                                      },
                                         SecondGrandChild = new GrandChildTarget()
                                                            {
                                                                Number = 2
                                                            }
                                     }
                         };

            ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.Number)
                .ToExpression<Target>().Compile()(target)
                .ShouldEqual(1);

            ReflectionHelper.GetAccessor<Target>(x => x.Child.SecondGrandChild.Number)
                .ToExpression<Target>().Compile()(target)
                .ShouldEqual(2);
        }

        [Test]
        public void property_chain_equals()
        {
            Accessor chain1 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain2 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain3 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild);
            Accessor chain4 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            Accessor chain5 = ReflectionHelper.GetAccessor<Target>(t => t.Child.SecondGrandChild.BirthDay);
            
            chain1.ShouldEqual(chain2);
            chain1.ShouldNotEqual(chain3);
            chain1.ShouldNotEqual(chain4);
            chain1.ShouldNotEqual(chain5);
            chain1.Equals(null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)chain1).ShouldBeTrue();
            ((PropertyChain)chain1).Equals(1).ShouldBeFalse();
            chain1.ShouldEqual(chain1);

        }

        [Test]
        public void propertyChain_hashcode()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.GetHashCode().ShouldNotEqual(0);
        }

        [Test]
        public void propertyChain_can_get_inner_property()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.InnerProperty.ShouldBeTheSameAs(typeof(ChildTarget).GetProperty("Age"));
        }

        [Test]
        public void propertyChain_can_get_child_accessor()
        {
            Accessor expected = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldEqual(expected);
        }

        [Test]
        public void propertyChain_can_get_the_name()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay).Name.ShouldEqual(
                "ChildGrandChildBirthDay");
        }

        [Test]
        public void propertyChain_can_get_owner_type()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).OwnerType.ShouldEqual(typeof(ChildTarget));
        }

        [Test]
        public void propertyChain_can_get_properties_names()
        {
            string[] names = ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).PropertyNames;
            names.ShouldHaveCount(2);
            names.ShouldHave(n => n == "Child");
            names.ShouldHave(n => n == "Age");
        }

        [Test]
        public void propertyChain_toString_returns_graph()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name.Length).
                ToString().ShouldEqual("FubuCore.Testing.Reflection.PropertyChainTester+TargetChild.GrandChild.Name");
        }

        [Test]
        public void PropertyChainCanGetPropertyHappyPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChild = new GrandChildTarget
                                                      {
                                                          BirthDay = DateTime.Today
                                                      }
                                     }
                         };
            _chain.GetValue(target).ShouldEqual(DateTime.Today);
        }

        [Test]
        public void PropertyChainCanSetPRopertyHappyPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChild = new GrandChildTarget
                                                      {
                                                          BirthDay = DateTime.Today
                                                      }
                                     }
                         };
            _chain.SetValue(target, DateTime.Today.AddDays(1));

            target.Child.GrandChild.BirthDay.ShouldEqual(DateTime.Today.AddDays(1));
        }

        [Test]
        public void PropertyChainGetPropertyReturnsNullForSadPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget()
                         };
            _chain.GetValue(target).ShouldBeNull();
        }

        [Test]
        public void PropertyChainReturnsInnerMostFieldName()
        {
            _chain.FieldName.ShouldEqual("BirthDay");
        }

        [Test]
        public void PropertyChainReturnsInnerMostPropertyType()
        {
            _chain.PropertyType.ShouldEqual(typeof(DateTime));
        }

        [Test]
        public void CollectionIndexingPropertyAccessWorks()
        {
            Expression<Func<Target, object>> expression = x => x.Child.GrandChildren[0].Name;
            var accessor = expression.ToAccessor();

            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChildren = { new GrandChildTarget { Name = "Bob" } }
                                     }
                         };

            accessor.GetValue(target).ShouldEqual("Bob");
            accessor.Name.ShouldEqual("ChildGrandChildren[0]Name");
        }
    }
}