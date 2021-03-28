\ Copyright 2021 quorum <micjph@protonmail.com> and contributors
\ 
\ This file is part of hc800dump.
\ 
\ hc800dump is free software: you can redistribute it and/or modify
\ it under the terms of the GNU General Public License as published by
\ the Free Software Foundation, either version 3 of the License, or
\ (at your option) any later version.
\ 
\ hc800dump is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.
\ 
\ You should have received a copy of the GNU General Public License
\ along with hc800dump.  If not, see <http://www.gnu.org/licenses/>.


\ ---------------------------------------------------------------------------
\ Dump HC800 executable files
\ ---------------------------------------------------------------------------

DECIMAL 0 CONSTANT HUNK_MMU
DECIMAL 1 CONSTANT HUNK_END
DECIMAL 2 CONSTANT HUNK_DATA


: open-input ( c-addr u -- fd iow )
	r/o bin open-file
;


VARIABLE ReadBuffer
: read-char ( fd -- c )
	ReadBuffer 1 rot read-file throw
	1 <> throw
	ReadBuffer @
;


: expect-char ( c fd -- )
	read-char <> throw
;


: dump-prologue ( fd -- )
	>r
	'U' r@ expect-char
	'C' r> expect-char
	." Prologue: UC (correct)"
;


: dump-bank-size ( u -- )
	1 + 16 * . ." KiB"
;


: file-skip ( u fd -- )
	>r
	dup 0<
	r@ file-position throw 
	d+	\ new position
	r> reposition-file throw
;


: dump-hex-byte ( n -- )
	0 <# HEX # # DECIMAL #> type
;


: dump-four-banks ( fd -- )
	4 0 ?DO
		dup read-char
		dump-hex-byte
		space
	LOOP
	drop
;


: dump-segment ( c-addr u u fd -- )
	>r >r 2dup

	2 spaces type space ." upper bank size: " r> dump-bank-size cr
	2 spaces type space ." banks: " r> dump-four-banks
;


: dump-mmu-hunk ( fd -- )
	>r		\ save fd

	r@ read-char	\ MMU config bits, kept on the stack throughout function

	2 spaces ." Harvard: "
	dup 1 and IF ." yes" ELSE ." no" THEN cr

	dup 2 rshift 3 and	\ code bank MMU config bits
	s" Code" rot r@ dump-segment

	dup 1 and IF	\ If Harvard architecture, show data bank configuration
		dup 4 rshift 3 and	\ data bank MMU config bits
		s" Data" rot r@ dump-segment
	ELSE
		4 r@ file-skip
	ENDIF

	rdrop
;


: dump-data-hunk ( n fd -- )
	dup read-char	\ MMU config bits, kept on the stack throughout function

	2 spaces ." Bank: " ." $" dump-hex-byte

	swap -1 + swap file-skip
;


: dump-hunk-type ( n -- )
	CASE
		HUNK_MMU OF ." HUNK_MMU, " ENDOF
		HUNK_END OF ." HUNK_END, " ENDOF
		HUNK_DATA OF ." HUNK_DATA, " ENDOF
		1 throw
	ENDCASE
;


: file-read-word ( fd -- n )
	dup read-char swap read-char 8 lshift or
;


: dump-hunk ( fd -- )
	>r

	r@ read-char dup
	dump-hunk-type

	r@ file-read-word dup
	." size " 0 <# HEX # # # # DECIMAL '$' HOLD #> type

	r@ file-position throw
	." , image location " throw 0 <# HEX # # # # # # '$' HOLD DECIMAL #> type

	swap CASE
		HUNK_MMU OF cr drop r> dump-mmu-hunk ENDOF
		HUNK_END OF r> file-skip ENDOF
		HUNK_DATA OF cr r> dump-data-hunk ENDOF
		1 throw
	ENDCASE
;


: dump-hunks ( fd -- )
	>r
	BEGIN
		r@ file-position throw
		r@ file-size throw
		d<
	WHILE
		r@ dump-hunk cr cr
	REPEAT
	rdrop
;


: dump-header ( fd -- )
	dup dump-prologue cr cr
;


: dump-fd ( fd -- )
	dump-header
	dump-hunks
;


: dump-name ( c-addr u -- )
	open-input 0= IF
		dump-fd
	ELSE
		." File does not exist" cr
	THEN
;
