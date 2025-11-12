# deploy

```bash
PACKAGE_VERSION=v1.0.2
helm --kubeconfig chainflip-kubeconfig.yaml package helm --version $PACKAGE_VERSION --app-version $PACKAGE_VERSION
helm --kubeconfig chainflip-kubeconfig.yaml upgrade swappy swappy-$PACKAGE_VERSION.tgz -i -n bots
```