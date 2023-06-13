@echo off
for /l %%a in (1,1,30) do (
   >>%%atextfile.txt echo bla bla bla
   >>%%atextfile.txt echo bla bla blog
   >>%%atextfile.txt echo bla bla bla
   >>%%atextfile.txt echo blom blom blom
)