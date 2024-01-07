namespace GenericRepository.OwnedPropertiesTree;

public record OwnedPropertiesTreeNode(string Name, Dictionary<string, OwnedPropertiesTreeNode> Children)
{
    public override string ToString()
    {
        return ToStringRecursive(this);
    }

    private static string ToStringRecursive(OwnedPropertiesTreeNode node, int level = 0)
    {
        return string.Join('\n',
            node.Children.Values.OrderBy(x => x.Name)
                .Select(x => ToStringRecursive(x, level + 1))
                .Prepend(new string(' ', level * 2) + node.Name)
        );
    }
}