![a](https://github.com/CumpsD/swappy/raw/main/assets/header.png "a")

# Swappy - [swappy.be](https://swappy.be)

> **A Discord bot to swap crypto currencies**

Built on [Chainflip](https://chainflip.io), a game-changing protocol for native cross-chain swaps.

Chainflip’s JIT AMM offers users low slippage swaps for native BTC and major chains. It’s decentralised, permissionless, composable, and powered by 150 independent validators.

Currently the following currencies are supported:

* Bitcoin (BTC)
* Polkadot (DOT)
* Ethereum (ETH)
* Chainflip (FLIP)
* USDC on Ethereum (USDC)
* USDT on Ethereum (USDT)
* Ethereum on Arbitrum (ETH)
* USDC on Arbitrum (USDC)
* SOL on Solana (SOL)
* USDC on Solana (USDC)
  
Simply type `/swap` on Discord and follow the provided steps.

* Select the asset to send.
* Select the asset to receive.
* Select the amount to swap.
* Receive an initial quote and provide the destination address.
* Receive a full quote to review and accept.
* Receive the deposit address where to send the source tokens to.
* Follow the progress on Chainflip until your swapped tokens arrive.

Users can add **Swappy** on to their Discord server via a one click install.
Click the link below.

**https://discord.com/api/oauth2/authorize?client_id=1200178122495111268&permissions=343597435968&scope=applications.commands+bot**

Once installed, the user can request a live quote using the bot. 

> Tip: Use `/quote` to request a live quote for the requested pair

![a](https://github.com/CumpsD/swappy/raw/main/assets/quote.png "a")

Next, you enter the amount for the swap and the coin/token pair.

Once submitted, a live quote will appear with the option to move forward with the swap.

![a](https://github.com/CumpsD/swappy/raw/main/assets/quote-result.png "a")

The user also has the option to request a swap directly instead of a quote.

> Tip: Use /swap to make a swap request

![a](https://github.com/CumpsD/swappy/raw/main/assets/swap-command.png "a")

**Swappy** will create a new thread to interact with. Just follow the prompts from here.

![a](https://github.com/CumpsD/swappy/raw/main/assets/step1.png "a")

Note: The *0x...* is a Swappy reference number for the bot. This is the only information that a user can use beside referencing on chain data to query about the swap request.

![a](https://github.com/CumpsD/swappy/raw/main/assets/step2.png "a")
![a](https://github.com/CumpsD/swappy/raw/main/assets/step3.png "a")
![a](https://github.com/CumpsD/swappy/raw/main/assets/step4.png "a")
![a](https://github.com/CumpsD/swappy/raw/main/assets/step5.png "a")

The last step to complete the swap is to send the submitted amount for the swap using your own wallet.

- Chainflip Explorer: [scan.chainflip.io](https://scan.chainflip.io/swaps?page=1&limit=50)

### Disclaimer

This Discord Bot, `Swappy`, is offered as an unofficial open-source tool ([github.com/CumpsD/swappy](https://github.com/CumpsD/swappy)) to perform non-custodial swaps over the [Chainflip Protocol](https://chainflip.io). 

The Broker it uses is operated independently of Chainflip Labs GmbH and it's associates, but instead by **David Cumps**, '_Developer_', with the on-chain address of [`0x6860efbced83aed83a483edd416a19ea45d88441`](https://etherscan.io/address/0x6860efbced83aed83a483edd416a19ea45d88441).

Versions of this bot may exist using modified code or other Broker services by other parties without the knowledge or explicit consent of the _Developer_.

The _Developer_ takes absolutely no responsibility for any losses incurred while using this service, it's code, or any tool, Discord server, or version of this bot. Users are encouraged to check that the Broker account using this bot or a version of its code is trustworthy, and before sending any swap funds, to verify the state of any deposit channel created for you using third-party block explorers.

By using this service, you acknowledge and agree that any and all losses incurred by you through this bot are your own responsibility.


