public interface IListViewCallbacks<TItemView>
	where TItemView : REListViewItem
{
	void ComponentCreated(TItemView instance);

	void ComponentDestroyed(TItemView instance);

	void ComponentActivated(TItemView instance);

	void ComponentCached(TItemView instance);
}
