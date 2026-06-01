import { useCallback, useEffect, useRef, useState } from 'react';
import { autocomplete, search } from '../api/userApi';
import type { User } from '../types/user';

const PAGE_SIZE = 25;

export function useSearch() {
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [results, setResults] = useState<User[]>([]);
  const [total, setTotal] = useState(0);
  const [from, setFrom] = useState(0);
  const [hasSearched, setHasSearched] = useState(false);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const skipAutocompleteRef = useRef(false);

  useEffect(() => {
    if (query.length < 2) {
      setSuggestions([]);
      return;
    }
    if (skipAutocompleteRef.current) {
      skipAutocompleteRef.current = false;
      return;
    }
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(async () => {
      const data = await autocomplete(query);
      setSuggestions(data);
    }, 250);
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, [query]);

  const runSearch = useCallback(async (q: string) => {
    setSuggestions([]);
    setHasSearched(true);
    setFrom(0);
    const data = await search(q, 0);
    setResults(data.users);
    setTotal(data.total);
  }, []);

  const loadMore = useCallback(async () => {
    const nextFrom = from + PAGE_SIZE;
    setIsLoadingMore(true);
    const data = await search(query, nextFrom);
    setResults(prev => [...prev, ...data.users]);
    setTotal(data.total);
    setFrom(nextFrom);
    setIsLoadingMore(false);
  }, [query, from]);

  const selectSuggestion = useCallback((name: string) => {
    skipAutocompleteRef.current = true;
    setQuery(name);
    runSearch(name);
  }, [runSearch]);

  const hasMore = hasSearched && results.length < total;

  return { query, setQuery, suggestions, results, total, hasSearched, runSearch, selectSuggestion, loadMore, hasMore, isLoadingMore };
}
