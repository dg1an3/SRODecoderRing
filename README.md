# SRODecoderRing
simple machine learning to infer rotation order from a matrix and Euler angles
uses a keras mlp to learn, then outputs to json csv to read in to an fsharp engine

https://keras.io/getting-started/sequential-model-guide/

And in the sledgehammer solution department--as mentioned in the previous post, the problem of mapping SRO meaning is insidious.  Confusion about patient positions (as in the this post) is bad enough.  So why not use machine learning here as well?

Given an SRO and an offset, train an ML model to recognize the input-output relationship.  Notebook link is here.

To turn the notebook in to a production service, we made the following adaptations:

Fable ElmishFatline front end on IPFS
Flask RESTful Web App on azure
Train through notebook
Weights in IPFS

A few contextual pieces of information that still need work:

 what about association to site?
 iView: Applying shift after first field--how to enter with TPO that won't associate with second field?
