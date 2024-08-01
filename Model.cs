using Jinaga;

[FactType("ToDo.List")]
public record List(string identifier)
{
    public Relation<Item> Items => Relation.Define(facts =>
        facts.OfType<Item>(item => item.list == this)
    );
}

[FactType("ToDo.Item")]
public record Item(List list, string description)
{
    public Relation<Completed> Completed => Relation.Define(facts =>
        facts.OfType<Completed>(completed => completed.item == this)
    );
    public Condition IsCompleted => Condition.Define(facts =>
        this.Completed.Any()
    );
}

[FactType("ToDo.Item.Completed")]
public record Completed(Item item);
