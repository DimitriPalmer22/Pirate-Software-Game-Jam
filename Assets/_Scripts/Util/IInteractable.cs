public interface IInteractable : IInterfacedObject
{
    public bool IsInteractable { get; }

    public void Interact(PlayerInteraction playerInteraction);

    public string InteractText(PlayerInteraction playerInteraction);
}