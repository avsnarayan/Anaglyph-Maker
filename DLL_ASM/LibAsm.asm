;********************************************************;
;          Assembler SIMD Anaglyph Algorithm             ;
;********************************************************;

; RCX - left image float pointer pixel array
; RDX - coords (start index and end index) int pointer
; R8 - filter anaglyph float pointer array
; R9 - right image float pointer pixel array

.DATA

maskLeft oword ?									; mask for left image
maskRight oword ?									; mask for right image

.CODE

AnaglyphAlgorithmAsm PROC

	mov			ebx, dword ptr[rdx]					; take start index from rdx
	mov			r11, rbx							; set it to r11
	mov			ebx, dword ptr[rdx + SIZEOF DWORD]	; take end index from rdx
	mov			r10, rbx							; set it to r10
	
	sub			r10, r11							; set counter
	mov			rdi, r10							; set it to rdi
	shr			edi, 2								; divide by 4 (4 values in each pixel rgba)

	mov			rax, 4h								; prepare multiplying by 4
	mul			r11									; multiply start index by 4 (rgba pixels 4 values)
	add			rcx, rax							; add to pixels offset table for left image float pointer
	add			r9, rax								; add to pixels offset table for right image float pointer

	xorps		xmm6, xmm6							; set 0 in register [0 0 0 0]
	pcmpeqd		xmm7, xmm7							; set 1 in register [1 1 1 1]
	movlhps		xmm6, xmm7							; move to get -> [0 0 1 1]
	movapd		[oword ptr maskLeft], xmm6			; set to maskLeft (mask for left image)

	xorps		xmm6, xmm6							; set 0 in register [0 0 0 0]
	movhlps		xmm6, xmm7							; move to get -> [1 1 0 0]
	movapd		[oword ptr maskRight], xmm6			; set to maskRight (mask for right image)

anaglyphLoop:
	cmp			edi, 0h								; check if the counter is 0, if so exit the loop					
	je			done								; exit the loop

	movdqu		xmm0, oword ptr[rcx]				; get pixel from left image to register [b g r a]
	movdqu		xmm2, oword ptr[r8]					; get red color filter to register
	mulps		xmm0, xmm2							; multiple pixel by red filter
	
	movapd		xmm3, xmm0							; move to tmp register
	shufps		xmm3, xmm3, 0b						; ->[bx bx bx bx]

	movapd		xmm4, xmm0							; move to tmp register
	shufps		xmm4, xmm4, 10101010b				; ->[gx gx gx gx]

	movapd		xmm5, xmm0							; move to tmp register
	shufps		xmm5, xmm5,	1010101b				; ->[rx rx rx rx]

	addps		xmm3, xmm4							; add pixels colors
	addps		xmm3, xmm5							; add pixels colors ->[Ar, Ar, Ar, Ar]

	andps		xmm3, xmmword ptr [maskLeft]		; multiple red anaglyph pixel color by left mask to get ->[0 0 Ar Ar]
	movapd		xmm0, xmm3

	movdqu		xmm1, oword ptr[r9]					; get pixel from right image to register [b g r a]
	movdqu		xmm2, oword ptr[r8 + SIZEOF OWORD]	; get green color filter to register
	mulps		xmm1, xmm2							; multiple pixel by green filter

	movapd		xmm3, xmm1							; move to tmp register
	shufps		xmm3, xmm3, 0b						; ->[by by by by]

	movapd		xmm4, xmm1							; move to tmp register
	shufps		xmm4, xmm4, 10101010b				; ->[gy gy gy gy]

	movapd		xmm5, xmm1							; move to tmp register
	shufps		xmm5, xmm5,	1010101b				; ->[ry ry ry ry]

	addps		xmm3, xmm4							; add pixels colors
	addps		xmm3, xmm5							; add pixels colors ->[Ag, Ag, Ag, Ag]
	movapd		xmm6, xmm3							; save temporarily result into register

	movdqu		xmm1, oword ptr[r9]					; get pixel from right image to register [b g r a]
	movdqu		xmm2, oword ptr[r8+2*SIZEOF OWORD]	; get blue color filter to register
	mulps		xmm1, xmm2							; multiple pixel by blue filter

	movapd		xmm3, xmm1							; move to tmp register
	shufps		xmm3, xmm3, 0b						; ->[bz bz bz bz]

	movapd		xmm4, xmm1							; move to tmp register
	shufps		xmm4, xmm4, 10101010b				; ->[gz gz gz gz]

	movapd		xmm5, xmm1							; move to tmp register
	shufps		xmm5, xmm5,	1010101b				; ->[rz rz rz rz]

	addps		xmm3, xmm4							; add pixels colors
	addps		xmm3, xmm5							; add pixels colors ->[Ab Ab, Ab, Ab]
	movapd		xmm7, xmm3							; save temporarily result into register

	unpckhps	xmm6, xmm7							; ->[Ag, Ab, Ag, Ab]
	andps		xmm6, xmmword ptr [maskRight]		; multiple by right mask [Ag Ab 0 0]

	addps		xmm0, xmm6							; add left and right result [0 0 Ar Ar] + [Ag Ab 0 0] -> [Ag Ab Ar Ar]
	movdqu		oword ptr[rcx], xmm0				; replace left image pixel with anaglyph pixel

	add			rcx, 16								; add offset to left image float pointer 
	add			r9, 16								; add offset to right image float pointer
	sub			rdi, 1								; decrease counter
	jmp			anaglyphLoop						; go to next loop iteration
done:
	ret

AnaglyphAlgorithmAsm ENDP
END