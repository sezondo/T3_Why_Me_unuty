public static class PlayerCostManager
{
    public static int GetCurrentCost()
    {
        if (NetworkPlayer.Local != null)
        {
            return NetworkPlayer.Local.Cost;
        }
        return 0;
    }

    public static bool CanAfford(int amount)
    {
        if (NetworkPlayer.Local != null)
        {
            return NetworkPlayer.Local.Cost >= amount;
        }
        return false;
    }

    public static void SpendCost(int amount)
    {
        if (NetworkPlayer.Local != null)
        {
            NetworkPlayer.Local.Rpc_SpendCost(amount);
        }
    }
}
