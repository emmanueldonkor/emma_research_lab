# Research questions

## Q01

How stable are accuracy, balanced accuracy, and F1 across several fixed
train/test split seeds when the same tabular pipeline is used correctly?

## Q02

On random-label data, how different is the held-out score when feature
selection sees all samples before the split versus when it is fitted on the
training partition only?

## Q03

How different can a single fixed train/test split look from the distribution
across several fixed splits of the same safe pipeline?

## Q04

Which pieces of run metadata are sufficient to regenerate the recorded
experiment output on the same dependency versions?
