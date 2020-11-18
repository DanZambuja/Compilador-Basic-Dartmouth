ENTRY(_start)
SECTIONS
{
  .
  = 0x0;
  .text : { * (.text); }
  .data : {*(.data)}
	.bss : {*(.bss)}
	.data : {*(image.o)}
  
  . = ALIGN(8);
  . = . + 0x1000;
  stack_top = .;
  . = . + 0x1000;
  irq_stack_top = .;
  . = . + 0x1000;
  a_stack_top = .;
  . = . + 0x1000;
  b_stack_top = .;
  . = . + 0x1000;
  c_stack_top = .;
  . = . + 0x1000;
  d_stack_top = .;
  . = . + 0x1000;
  e_stack_top = .;
  . = . + 0x1000;
  f_stack_top = .;
  . = . + 0x1000;
  g_stack_top = .;
  . = . + 0x1000;
  h_stack_top = .;
  . = . + 0x1000;
  i_stack_top = .;
  . = . + 0x1000;
  j_stack_top = .;
  . = . + 0x1000;
  a_stack_top_svc = .;
  . = . + 0x1000;
  b_stack_top_svc = .;
  . = . + 0x1000;
  c_stack_top_svc = .;
  . = . + 0x1000;
  d_stack_top_svc = .;
  . = . + 0x1000;
  e_stack_top_svc = .;
  . = . + 0x1000;
  f_stack_top_svc = .;
  . = . + 0x1000;
  g_stack_top_svc = .;
  . = . + 0x1000;
  h_stack_top_svc = .;
  . = . + 0x1000;
  i_stack_top_svc = .;
  . = . + 0x1000;
  j_stack_top_svc = .;
}
