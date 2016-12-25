(define nil?
  (lambda (x) (eq? x nil)))

(define map
  (lambda (lst func)
    (cond (((nil? lst) nil)
           (true (cons (func (car lst))
                       (map (cdr lst) func)
                       )
                 )
           )
          )
    )
  )

(define one_two_three_four
  (cons 1 (cons 2 (cons 3 (cons 4 nil))))
