<div align="center">
  <img width="138" alt="Screenshot 2024-02-08 at 8 30 35 AM" src="https://github.com/Proskhmer08/swappy/assets/35155781/2ee6e4f3-5000-47df-9a15-98e75340d2bd">
	<h1>Swappy-Bot</h1>
	<p>
		<b>A Discord bot to swap crypto currencies</b>
	</p>
	<br>
</div>



Built on [Chainflip](https://chainflip.io), a game-changing protocol for native cross-chain swaps.

Chainflip’s JIT AMM offers users low slippage swaps for native BTC and major chains. It’s decentralised, permissionless, composable, and powered by 150 independent validators.

Currently the following currencies are supported:

* Bitcoin (BTC)
* Polkadot (DOT)
* Ethereum (ETH)
* Chainflip (FLIP)
* USDC on Ethereum (USDC)

Simply type `/swap` on Discord and follow the provided steps.

* Select the asset to send.
* Select the asset to receive.
* Select the amount to swap.
* Receive an initial quote and provide the destination address.
* Receive a full quote to review and accept.
* Receive the deposit address where to send the source tokens to.
* Follow the progress on Chainflip until your swapped tokens arrive.

Users can add Chainflip-Insights onto their Discord server via a one click install.
Click the link below. <br> https://discord.com/api/oauth2/authorize?client_id=1200178122495111268&permissions=343597435968&scope=applications.commands+bot
<br>
<br>
Once installed, user can request a live quote using the bot. 
> Tip: Use /quote to request a live quote for the requested pair
<br>
<img width="867" alt="Screenshot 2024-02-07 at 3 26 04 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/ec6b33ca-291c-4302-9318-194d6979bdb4">

Next, you enter the amount for the swap and the coin/token pair.
<br>
<br>
<br>
Once submitted, a live quote will appear with the option to move forward with the swap.
<img width="867" alt="Screenshot 2024-02-07 at 3 29 57 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/b3d3015c-178f-41ad-b8c0-8568315a8d47">
<br>
<br>
<br>
User also has the option to request a swap directly instead of a quote.
> Tip: Use /swap to make a swap request
<br>
<img width="867" alt="Screenshot 2024-02-07 at 3 40 13 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/b5ffe2ab-a226-44a9-a5d5-fe54c7c07539">
<br>
<br>
The discord bot will create a new thread to interact with. Just follow the prompt from here.
<img width="867" alt="Screenshot 2024-02-07 at 3 40 53 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/e239735a-15f3-4910-8d35-7a77ffd9cc93">
<br>
<br>
Note: The 0x...is a Chainflip-Insights swap reference number to the bot. This is the only information that a user can use beside referencing on chain data to querry about the swap request.
<br>
<br>
<img width="876" alt="Screenshot 2024-02-07 at 3 41 11 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/495037d7-b8d7-49ea-8165-87adceaf96fd">
<br>
<img width="853" alt="Screenshot 2024-02-07 at 3 48 35 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/c30daa9c-85ab-4e63-bea3-1555c9440f39">
<br>
<img width="647" alt="Screenshot 2024-02-07 at 3 50 40 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/0ec9e7b9-5f34-4ef9-80b1-bcad7a161b1b">
<br>
<img width="596" alt="Screenshot 2024-02-07 at 3 51 25 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/f9e5625d-7ebe-48a4-94c7-5b48f787f411">
<br>
<img width="779" alt="Screenshot 2024-02-07 at 3 57 38 PM" src="https://github.com/Proskhmer08/chainflip-insights/assets/35155781/f3adbe96-e6b7-4fae-9647-c6aa154d8895">
<br>
Last step to complete the swap is to send the submitted amount for the swap using your own wallet.

- Chainflip Explorer: [scan.chainflip.io](https://scan.chainflip.io/swaps?page=1&limit=50)
<br>

### Disclaimer

This Discord Bot, `swappy!`, is offered as an unofficial open-source tool ([github.com/CumpsD/swappy](https://github.com/CumpsD/swappy)) to perform non-custodial swaps over the [Chainflip Protocol](https://chainflip.io). 

The Broker it uses is operated independently of Chainflip Labs GmbH and it's associates, but instead by **David Cumps**, '_Developer_', with the on-chain address of [`0x6860efbced83aed83a483edd416a19ea45d88441`](https://etherscan.io/address/0x6860efbced83aed83a483edd416a19ea45d88441).

Versions of this bot may exist using modified code or other Broker services by other parties without the knowledge or explicit consent of the _Developer_.

The _Developer_ takes absolutely no responsibility for any losses incurred while using this service, it's code, or any tool, Discord server, or version of this bot. Users are encouraged to check that the Broker account using this bot or a version of its code is trustworthy, and before sending any swap funds, to verify the state of any deposit channel created for you using third-party block explorers.

By using this service, you acknowledge and agree that any and all losses incurred by you through this bot are your own responsibility.


