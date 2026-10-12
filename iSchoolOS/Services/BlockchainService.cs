using System.Security.Cryptography;
using System.Text;
using iSchoolOS.Models;

namespace iSchoolOS.Services;

public class BlockchainService
{
    private readonly List<BlockchainBlock> chain = new();

    public BlockchainService()
    {
        AddGenesisBlock();
    }

    private void AddGenesisBlock()
    {
        var block = new BlockchainBlock
        {
            Index = 0,
            Timestamp = DateTime.Now,
            Transaction = "Genesis Block",
            PreviousHash = "0"
        };
        block.Hash = CalculateHash(block);
        chain.Add(block);
    }

    public BlockchainBlock AddTransaction(string transaction)
    {
        var previous = chain[^1];
        var block = new BlockchainBlock
        {
            Index = previous.Index + 1,
            Timestamp = DateTime.Now,
            Transaction = transaction,
            PreviousHash = previous.Hash
        };
        block.Hash = CalculateHash(block);
        chain.Add(block);
        return block;
    }

    private string CalculateHash(BlockchainBlock block)
    {
        string input = $"{block.Index}{block.Timestamp}{block.Transaction}{block.PreviousHash}";
        using SHA256 sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public bool VerifyChain()
    {
        for (int i = 1; i < chain.Count; i++)
        {
            if (chain[i].PreviousHash != chain[i - 1].Hash)
            {
                return false;
            }
        }
        return true;
    }

    public BlockchainBlock GetLatestBlock()
    {
        return chain[^1];
    }
}