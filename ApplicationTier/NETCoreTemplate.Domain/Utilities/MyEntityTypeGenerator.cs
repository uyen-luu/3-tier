using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace NETCoreTemplate.Domain.Utilities
{
    class MyEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        public MyEntityTypeGenerator([NotNull] IAnnotationCodeGenerator annotationCodeGenerator, [NotNull] ICSharpHelper cSharpHelper) : base(annotationCodeGenerator, cSharpHelper)
        {
        }

        public override string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            string code = base.WriteCode(entityType, @namespace, useDataAnnotations);

            var oldString = "public partial class " + entityType.Name;
            var newString = "public partial class " + entityType.Name + " : EntityBase<int>";

            var oldId = "public int Id { get; set; }";

            return code.Replace(oldString, newString).Replace(oldId, string.Empty);
        }
    }
}
