using FubuMVC.Core.Assets;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetOrderingAndQueryingAcceptanceTests
    {
        [Test]
        public void simple_queries()
        {
            AssetGraphScenario.For("simple query", @"
requesting A, B
should return A, B
");
        }

        [Test]
        public void order_by_name_in_absence_of_any_other_criteria()
        {
            AssetGraphScenario.For("simple query", @"
requesting B, A
should return A, B
");
        }

        [Test]
        public void dependencies_between_files()
        {
            AssetGraphScenario.For("dependencies between files", @"
If the asset configuration is
A requires B,C,D
C requires B,E
E requires F
F requires D
G requires H,I
I requires H,J

# One deep dependency
requesting F
should return D,F

# Two deep dependency
requesting E
should return D,F,E

# Three deep dependency
requesting A
should return B,D,F,E,C,A

# 3 deep in parallel
requesting A,G
should return B, D, H, J, F, I, E, G, C, A

");
        }


        [Test]
        public void dependencies_through_aliases()
        {
            AssetGraphScenario.For("dependencies between files", @"
If the script graph is configured as
A requires B,C,D
C requires B,E
E requires F
F requires D
G requires H,I
I requires H,J
A is A.js
B is B.js

# One deep dependency
requesting F
should return D,F

# Two deep dependency
requesting E
should return D,F,E

# Three deep dependency
requesting A
should return B.js,D,F,E,C,A.js

# deep in parallel
requesting A,G
should return B.js, D, H, J, F, I, E, G, C, A.js
");
        }


        [Test]
        public void mixed_queries()
        {
            AssetGraphScenario.For("mixed queries", @"
If the script graph is configured as
1 includes A,B,C
2 includes C,D
3 includes 1,E
D requires D1,D2
3 requires 4
4 includes jquery,jquery.validation
Combo includes 1,2
C-1 extends C
crud includes crudForm.js,validation.js
A requires crud

# Query for only one set with no dependencies
requesting 1
should return B, C, crudForm.js, validation.js, A, C-1

# Query for a set whose files have a dependency
requesting 2
should return C, D1, D2, C-1, D

# Fetch a set that references another set
requesting Combo
should return B, C, crudForm.js, D1, D2, validation.js, A, C-1, D

# Set with a dependency on another set
requesting 3
should return B, C, crudForm.js, E, jquery, jquery.validation, validation.js, A, C-1

# Multiple options
requesting 1,2,A,C
should return B, C, crudForm.js, D1, D2, validation.js, A, C-1, D
");
        }


        [Test]
        public void extensions_to_files()
        {
            AssetGraphScenario.For("extensions to files", @"
If the script graph is configured as
A requires B,C
D extends A
F extends B

# Single file should put its extension right behind it
requesting B
should return B,F

# Query for two files with extensions
requesting A
should return B, C, F, A, D
");
        }


        [Test]
        public void query_by_sets()
        {
            AssetGraphScenario.For("query by sets", @"
If the script graph is configured as
1 includes A,B,C
2 includes C,D
3 includes 1,E
D requires D1,D2
3 requires 4
4 includes jquery,jquery.validation
Combo includes 1,2

# Query for only one set with no dependencies
requesting 1
should return A,B,C

# Query for a set whose files have a dependency
requesting 2
should return C,D1,D2,D

# Fetch a set that references another set
requesting Combo
should return A, B, C, D1, D2, D

# Set with a dependency on another set
requesting 3
should return A, B, C, E, jquery, jquery.validation
");
        }

    }


}