# deploy

```bash
helm --kubeconfig chainflip-kubeconfig.yaml package helm --version $PACKAGE_VERSION --app-version $PACKAGE_VERSION
helm --kubeconfig chainflip-kubeconfig.yaml upgrade swappy swappy-$PACKAGE_VERSION.tgz -i -n bots
```