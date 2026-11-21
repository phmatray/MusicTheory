using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SiteGen;

/// <summary>API pages for every public type of MusicTheory and MusicTheory.Guitar, read from reflection and the XML docs.</summary>
internal sealed partial class ApiReference
{
    private static readonly Assembly[] Assemblies = [typeof(MusicTheory.Note).Assembly, typeof(MusicTheory.Guitar.Tuning).Assembly];

    private readonly Dictionary<string, XElement> _docs = new();
    private readonly NullabilityInfoContext _nullability = new();

    public IReadOnlyList<Type> Types { get; }

    public ApiReference()
    {
        foreach (var assembly in Assemblies)
        {
            var xml = Path.Combine(AppContext.BaseDirectory, assembly.GetName().Name + ".xml");
            foreach (var member in XDocument.Load(xml).Descendants("member"))
                _docs[member.Attribute("name")!.Value] = member;
        }
        Types = Assemblies.SelectMany(a => a.GetExportedTypes()).OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToList();
    }

    public static string FileName(Type type) => type.FullName!.Replace('`', '-');

    /// <summary>The first sentence of a type's summary, as plain text.</summary>
    public string Summary(Type type) => Regex.Replace(Text(Doc("T:" + type.FullName)?.Element("summary")), "<[^>]+>", "").Split(". ")[0].TrimEnd('.') + ".";

    /// <summary>Wraps every <c>TypeName</c> in inline code that names a public type in a link to its page.</summary>
    public string LinkTypes(string html) => InlineCode().Replace(html, m =>
    {
        var type = Types.FirstOrDefault(t => t.Name == m.Groups[1].Value);
        return type is null ? m.Value : $"<a href=\"../api/{FileName(type)}.html\"><code>{m.Groups[1].Value}</code></a>";
    });

    public string IndexPage()
    {
        var sb = new StringBuilder("<h1>API reference</h1><p class=\"lede\">Generated from the XML documentation of the published packages, so it always matches the code.</p>");
        foreach (var ns in Types.GroupBy(t => t.Namespace))
        {
            sb.Append($"<h2 id=\"{ns.Key!.Replace('.', '-').ToLowerInvariant()}\">{ns.Key}</h2><dl class=\"api-index\">");
            foreach (var type in ns)
                sb.Append($"<dt><a href=\"{FileName(type)}.html\"><code>{Name(type)}</code></a> <span class=\"kind\">{Kind(type)}</span></dt><dd>{WebUtility.HtmlEncode(Summary(type))}</dd>");
            sb.Append("</dl>");
        }
        return sb.ToString();
    }

    public string TypePage(Type type)
    {
        var doc = Doc("T:" + type.FullName);
        var sb = new StringBuilder($"<p class=\"crumb\"><a href=\"index.html\">API</a> / {type.Namespace}</p>");
        sb.Append($"<h1><code>{WebUtility.HtmlEncode(Name(type))}</code> <span class=\"kind\">{Kind(type)}</span></h1>");
        sb.Append(Block(doc, "summary", "lede")).Append(Block(doc, "remarks", "remarks"));

        if (type.IsEnum)
        {
            sb.Append("<h2 id=\"values\">Values</h2><table class=\"api-values\"><thead><tr><th>Name</th><th>Value</th><th>Meaning</th></tr></thead><tbody>");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                sb.Append($"<tr><td><code>{field.Name}</code></td><td>{Convert.ToInt64(field.GetRawConstantValue())}</td><td>{Text(Doc($"F:{type.FullName}.{field.Name}")?.Element("summary"))}</td></tr>");
            return sb.Append("</tbody></table>").ToString();
        }

        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        Section(sb, "Constructors", type.GetConstructors().Where(c => !IsHidden(c)).Select(c => (Signature(c), (MemberInfo)c, Id(c))));
        Section(sb, "Properties", type.GetProperties(flags).Where(p => !IsHidden(p)).Select(p => (Signature(p), (MemberInfo)p, $"P:{type.FullName}.{p.Name}")));
        Section(sb, "Fields", type.GetFields(flags).Where(f => !IsHidden(f)).Select(f => (Signature(f), (MemberInfo)f, $"F:{type.FullName}.{f.Name}")));
        Section(sb, "Methods", type.GetMethods(flags).Where(m => !m.IsSpecialName && !IsHidden(m)).OrderBy(m => m.Name).Select(m => (Signature(m), (MemberInfo)m, Id(m))));
        return sb.ToString();
    }

    private void Section(StringBuilder sb, string title, IEnumerable<(string Signature, MemberInfo Member, string Id)> members)
    {
        var list = members.ToList();
        if (list.Count == 0)
            return;
        sb.Append($"<h2 id=\"{title.ToLowerInvariant()}\">{title}</h2>");
        foreach (var (signature, member, id) in list)
        {
            var doc = Doc(id) ?? InheritedDoc(member);
            sb.Append($"<section class=\"member\"><pre class=\"signature\"><code>{Highlight.CSharp(signature)}</code></pre>");
            sb.Append(Block(doc, "summary", "summary"));
            var parameters = doc?.Elements("param").ToList() ?? [];
            if (parameters.Count > 0)
                sb.Append("<dl class=\"params\">").Append(string.Concat(parameters.Select(p => $"<dt><code>{p.Attribute("name")!.Value}</code></dt><dd>{Text(p)}</dd>"))).Append("</dl>");
            if (doc?.Element("returns") is { } returns)
                sb.Append($"<p class=\"returns\"><span>Returns</span> {Text(returns)}</p>");
            foreach (var exception in doc?.Elements("exception") ?? [])
                sb.Append($"<p class=\"throws\"><span>Throws</span> <code>{Cref(exception.Attribute("cref")!.Value)}</code> {Text(exception)}</p>");
            sb.Append("</section>");
        }
    }

    // <inheritdoc/> on overrides (ToString, Equals): show the overridden member's text when there is one.
    private XElement? InheritedDoc(MemberInfo member) => member is MethodInfo m && m.GetBaseDefinition() != m ? Doc(Id(m.GetBaseDefinition())) : null;

    private static bool IsHidden(MemberInfo member) =>
        member.IsDefined(typeof(CompilerGeneratedAttribute)) || member.Name is "EqualityContract" or "<Clone>$" or "Deconstruct" or "PrintMembers";

    private XElement? Doc(string id) => _docs.GetValueOrDefault(id);

    private string Block(XElement? doc, string element, string cssClass) =>
        doc?.Element(element) is { } e ? $"<div class=\"{cssClass}\">{Text(e)}</div>" : "";

    /// <summary>XML doc content to HTML: c, see, paramref, para.</summary>
    private string Text(XElement? element)
    {
        if (element is null)
            return "";
        var sb = new StringBuilder();
        foreach (var node in element.Nodes())
        {
            sb.Append(node switch
            {
                XText text => WebUtility.HtmlEncode(text.Value),
                XElement { Name.LocalName: "c" } c => $"<code>{WebUtility.HtmlEncode(c.Value)}</code>",
                XElement { Name.LocalName: "paramref" or "typeparamref" } p => $"<code>{p.Attribute("name")!.Value}</code>",
                XElement { Name.LocalName: "see" } see when see.Attribute("langword") is { } word => $"<code>{word.Value}</code>",
                XElement { Name.LocalName: "see" } see => SeeLink(see.Attribute("cref")!.Value),
                XElement { Name.LocalName: "para" } para => $"<br><br>{Text(para)}",
                XElement other => Text(other),
                _ => "",
            });
        }
        return Regex.Replace(sb.ToString().Trim(), @"\s+", " ");
    }

    private string SeeLink(string cref)
    {
        var typeName = Regex.Replace(cref[2..], @"\(.*", "");
        var type = Types.FirstOrDefault(t => t.FullName == typeName) ??
                   Types.FirstOrDefault(t => typeName.StartsWith(t.FullName + "."));
        var text = $"<code>{WebUtility.HtmlEncode(Cref(cref))}</code>";
        return type is null ? text : $"<a href=\"{FileName(type)}.html\">{text}</a>";
    }

    private static string Cref(string cref) => Regex.Replace(Regex.Replace(cref[2..], @"\(.*", ""), @"^(MusicTheory\.Guitar\.Blazor\.|MusicTheory\.Guitar\.|MusicTheory\.|System\.)", "");

    // ── Signatures ──────────────────────────────────────────────────────────────

    private static string Kind(Type t) =>
        t.IsEnum ? "enum" : t.IsInterface ? "interface" : t.IsValueType ? "struct" :
        t.IsAbstract && t.IsSealed ? "static class" : t.GetMethod("<Clone>$") is not null ? "record" : "class";

    private static string Name(Type t) => t.IsGenericType ? $"{t.Name[..t.Name.IndexOf('`')]}<{string.Join(", ", t.GetGenericArguments().Select(Name))}>" : t.Name;

    private string Signature(ConstructorInfo c) => $"{Name(c.DeclaringType!)}({Parameters(c)})";

    private string Signature(PropertyInfo p) =>
        $"{(p.GetMethod?.IsStatic == true ? "static " : "")}{TypeName(p.PropertyType, _nullability.Create(p))} {p.Name} {{ get; {(p.SetMethod?.IsPublic == true ? (p.SetMethod.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit)) ? "init; " : "set; ") : "")}}}";

    private string Signature(FieldInfo f) =>
        $"{(f.IsLiteral ? "const " : f.IsStatic ? "static readonly " : "")}{TypeName(f.FieldType, _nullability.Create(f))} {f.Name}{(f.IsLiteral ? " = " + Literal(f.GetRawConstantValue()) : "")}";

    private string Signature(MethodInfo m) =>
        $"{(m.IsStatic ? "static " : "")}{TypeName(m.ReturnType, _nullability.Create(m.ReturnParameter))} {m.Name}({Parameters(m)})";

    private string Parameters(MethodBase m) => string.Join(", ", m.GetParameters().Select(p =>
        (p.IsOut ? "out " : "") + TypeName(p.IsOut ? p.ParameterType.GetElementType()! : p.ParameterType, _nullability.Create(p)) + " " + p.Name +
        (p.HasDefaultValue ? " = " + Literal(p.DefaultValue) : "")));

    private static string Literal(object? value) => value switch
    {
        null => "null",
        string s => $"\"{s}\"",
        bool b => b ? "true" : "false",
        double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Enum e => $"{e.GetType().Name}.{e}",
        _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)!,
    };

    private static readonly Dictionary<Type, string> Aliases = new()
    {
        [typeof(void)] = "void", [typeof(int)] = "int", [typeof(double)] = "double", [typeof(string)] = "string",
        [typeof(bool)] = "bool", [typeof(char)] = "char", [typeof(object)] = "object", [typeof(long)] = "long",
    };

    private static string TypeName(Type t, NullabilityInfo? nullability = null)
    {
        var suffix = nullability?.ReadState == NullabilityState.Nullable && !t.IsValueType ? "?" : "";
        if (Nullable.GetUnderlyingType(t) is { } underlying)
            return TypeName(underlying) + "?";
        if (Aliases.TryGetValue(t, out var alias))
            return alias + suffix;
        if (t.IsArray)
            return TypeName(t.GetElementType()!, nullability?.ElementType) + "[]" + suffix;
        if (t.IsGenericType && t.FullName?.StartsWith("System.ValueTuple") == true)
            return $"({string.Join(", ", t.GetGenericArguments().Select(a => TypeName(a)))})" + suffix;
        if (t.IsGenericType)
        {
            var args = t.GetGenericArguments().Select((a, i) => TypeName(a, nullability?.GenericTypeArguments.ElementAtOrDefault(i)));
            return $"{t.Name[..t.Name.IndexOf('`')]}<{string.Join(", ", args)}>" + suffix;
        }
        return t.Name + suffix;
    }

    // ── XML doc ids ─────────────────────────────────────────────────────────────

    private static string Id(MethodBase m)
    {
        var name = m is ConstructorInfo ? "#ctor" : m.Name;
        var parameters = m.GetParameters();
        var arity = m is MethodInfo { IsGenericMethod: true } g ? "``" + g.GetGenericArguments().Length : "";
        return $"M:{m.DeclaringType!.FullName}.{name}{arity}" + (parameters.Length == 0 ? "" : "(" + string.Join(",", parameters.Select(p => IdType(p.ParameterType))) + ")");
    }

    private static string IdType(Type t)
    {
        if (t.IsByRef)
            return IdType(t.GetElementType()!) + "@";
        if (t.IsArray)
            return IdType(t.GetElementType()!) + "[]";
        if (t.IsGenericParameter)
            return (t.DeclaringMethod is null ? "`" : "``") + t.GenericParameterPosition;
        if (t.IsGenericType)
        {
            var definition = t.GetGenericTypeDefinition().FullName!;
            return definition[..definition.IndexOf('`')] + "{" + string.Join(",", t.GetGenericArguments().Select(IdType)) + "}";
        }
        return t.FullName!.Replace('+', '.');
    }

    [GeneratedRegex(@"(?<![\w>])<code>([A-Z][A-Za-z0-9]+)</code>")]
    private static partial Regex InlineCode();
}
