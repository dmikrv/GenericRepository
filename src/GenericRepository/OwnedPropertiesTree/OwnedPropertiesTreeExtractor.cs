using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.OwnedPropertiesTree;

public class OwnedPropertiesTreeExtractor
{
    private const string IncludeMethodName = nameof(EntityFrameworkQueryableExtensions.Include);
    private const string ThenIncludeMethodName = nameof(EntityFrameworkQueryableExtensions.ThenInclude);
    private static readonly Type EfCoreExtensionsClass = typeof(EntityFrameworkQueryableExtensions);
    private static readonly string[] TargetMethodNames = { IncludeMethodName, ThenIncludeMethodName };

    public static IEnumerable<QueryableIncludeNode> ExtractIncludesTree<T>(IQueryable<T> query)
    {
        var node = query.Expression;
        while (node is MethodCallExpression methodCall)
        {
            var method = methodCall.Method;

            // We are interested only in .Include() or .ThenInclude() methods here
            if (method.DeclaringType == EfCoreExtensionsClass && TargetMethodNames.Contains(method.Name))
            {
                var argument = methodCall.Arguments[1];
                var lambda = (LambdaExpression)argument.GetType().GetProperty("Operand")!.GetValue(argument)!;
                var memberExpression = (MemberExpression)lambda.Body;
                var propertyName = memberExpression.Member.Name;
                yield return new QueryableIncludeNode(propertyName, method.Name == IncludeMethodName);
            }

            node = methodCall.Arguments[0];
        }
    }


    public OwnedPropertiesTreeNode ExtractOwnedPropertiesTreeNodes<T>(IQueryable<T> query)
    {
        var root = new OwnedPropertiesTreeNode("Root", new Dictionary<string, OwnedPropertiesTreeNode>());
        var currentIncludeStack = new Stack<QueryableIncludeNode>();

        foreach (var node in ExtractIncludesTree(query))
        {
            currentIncludeStack.Push(node);

            if (!node.IsRoot) continue;

            MergeTreeNodes(root, currentIncludeStack);
            currentIncludeStack.Clear();
        }

        return root;
    }

    private static void MergeTreeNodes(OwnedPropertiesTreeNode root, IEnumerable<QueryableIncludeNode> nodes)
    {
        foreach (var node in nodes)
            if (!root.Children.TryGetValue(node.Name, out var child))
            {
                var newNode = new OwnedPropertiesTreeNode(node.Name, new Dictionary<string, OwnedPropertiesTreeNode>());
                root.Children.Add(node.Name, newNode);
                root = newNode;
            }
            else
            {
                root = child;
            }
    }
}