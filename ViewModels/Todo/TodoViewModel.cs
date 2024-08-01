namespace ViewModels.Todo;

using Jinaga;

public class ItemViewModel(Item item)
{
    public string Description { get; set; } = "";
    public bool IsCompleted { get; set; }

    public async Task SetCompleted(JinagaClient jinagaClient)
    {
        await jinagaClient.Fact(new Completed(item));
    }
}

public class TodoViewModel(JinagaClient jinagaClient, ILogger<TodoViewModel> logger)
{
    private IObserver? observer;

    public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();

    public async Task Load(Action stateHasChanged)
    {
        var itemsInList = Given<List>.Match((list, facts) =>
            list.Items.Select(item => new
            {
                Item = item,
                Description = item.description,
                Competed = facts.Observable(item.Completed)
            })
        );
        var list = await jinagaClient.Fact(new List("ToDo"));

        // Add some test items
        await jinagaClient.Fact(new Item(list, "Buy milk"));
        await jinagaClient.Fact(new Item(list, "Walk the dog"));
        var laundry = await jinagaClient.Fact(new Item(list, "Do the laundry"));
        await jinagaClient.Fact(new Completed(laundry));

        observer = jinagaClient.Watch(itemsInList, list, projection =>
        {
            var itemViewModel = new ItemViewModel(projection.Item)
            {
                Description = projection.Description,
                IsCompleted = false
            };

            projection.Competed.OnAdded(completed =>
            {
                logger.LogInformation("Item completed");
                itemViewModel.IsCompleted = true;
                stateHasChanged();
            });

            Items.Add(itemViewModel);
            stateHasChanged();

            return () =>
            {
                Items.Remove(itemViewModel);
                stateHasChanged();
            };
        });

        await observer.Loaded;
        logger.LogInformation("Loaded items in list");
    }

    public void Unload()
    {
        observer?.Stop();
    }

    public async Task SetCompleted(ItemViewModel itemViewModel)
    {
        await itemViewModel.SetCompleted(jinagaClient);
    }
}