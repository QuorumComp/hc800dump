include hc800dump.fs

:noname
	Defers 'cold \ do other initialization stuff (e.g., rehashing wordlists)
	next-arg 2dup 0 0 d<> IF 
		dump-name
	ELSE
		." Usage: hc800dump <hc800-executable>" cr
	THEN
	bye
; IS 'cold
